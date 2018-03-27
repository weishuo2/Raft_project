using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaftServerProperty))]
public class RaftRPCReceiver : MonoBehaviour
{
    private RaftServerProperty _serverProperty;
    private RaftStateController _serverStateController;
    private RaftRPCSender _rpcSender;

    private void Awake()
    {
        _serverProperty = GetComponent<RaftServerProperty>();
        _serverStateController = GetComponent<RaftStateController>();
        _rpcSender = GetComponent<RaftRPCSender>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Receive(collision.gameObject.GetComponent<RaftBaseRPCModel>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Receive(collision.gameObject.GetComponent<RaftBaseRPCModel>());
    }

    private void Receive(RaftBaseRPCModel rpcModel)
    {
        if (rpcModel == null || rpcModel.m_target != transform)
        {
            return;
        }

        // This server is not working
        if(!_serverProperty.enabled)
        {
            Destroy(rpcModel.gameObject);
            return;
        }


        // If leader's term > currentTerm, set currentTerm = leader's term, convert to follower
        if (rpcModel.m_term > _serverProperty.m_currentTerm)
        {
            _serverProperty.m_currentTerm = rpcModel.m_term;
            _serverStateController.m_currentState = _serverStateController.GetState("Follower State");
            _serverStateController.m_currentState.InitializeState(_serverProperty);
            _serverProperty.GetComponent<RaftServerEventMaster>().CallOnChangeTerm(_serverProperty.m_currentTerm);
        }

        switch (rpcModel.m_rpcType)
        {
            case RaftRPCType.AppendEntriesArgu:
                ProcessAppendEntries((RaftAppendEntriesArgus)rpcModel);
                break;
            case RaftRPCType.AppendEntriesReturn:
                ProcessAppendEntriesReturn((RaftAppendEntriesReturns)rpcModel);
                break;
            case RaftRPCType.RequestVoteArgu:
                ProcessRequestVote((RaftRequestVoteArgus)rpcModel);
                break;
            case RaftRPCType.RequestVoteReturn:
                ProcessRequestVoteReturn((RaftRequestVoteReturns)rpcModel);
                break;
        }

        Destroy(rpcModel.gameObject);
    }


    private void ProcessAppendEntries(RaftAppendEntriesArgus rpcModel)
    {
        var leader = RaftServerManager.Instance.GetServer(rpcModel.m_leaderId);
        if (leader == null) return;

        // If current state is Follower, update its timeout
        if (_serverStateController.m_stateType == RaftStateType.Follower)
        {
            _serverStateController.m_currentState.InitializeState(_serverProperty);
        }

        // Reply false if leader's term < currentTerm
        if (rpcModel.m_term < _serverProperty.m_currentTerm)
        {
            _rpcSender.SendAppendEntriesRPCReturn(_serverProperty.m_currentTerm, false, _serverProperty.m_serverId, leader.transform);
            return;
        }

        // Reply false if log doesn't contain an entry at prevLogIndex whose term matches prevLogTerm
        int logTerm = rpcModel.m_prevLogIndex == 0 ? 0 : _serverProperty.m_logs[rpcModel.m_prevLogIndex - 1].m_term;
        if (logTerm != rpcModel.m_prevLogTerm)
        {
            _rpcSender.SendAppendEntriesRPCReturn(_serverProperty.m_currentTerm, false, _serverProperty.m_serverId, leader.transform);
            return;
        }


        if ((rpcModel.m_entries != null) && (rpcModel.m_entries.Count > 0))
        {
            int matchIndex = 0;

            // If an existing entry conflicts with a new one (same index but different terms), 
            // delete the existing entry and all that follow it
            for (int i = rpcModel.m_prevLogIndex + 1; (i <= _serverProperty.m_logs.Count) && (matchIndex < rpcModel.m_entries.Count); i++, matchIndex++)
            {
                if (_serverProperty.m_logs[i - 1].m_term != rpcModel.m_entries[matchIndex].m_term)
                {
                    _serverProperty.m_logs.RemoveRange(i - 1, _serverProperty.m_logs.Count - i + 1);
                    break;
                }
            }

            // Append any new entries not already in the log
            var serverEventMaster = _serverProperty.GetComponent<RaftServerEventMaster>();
            while (matchIndex < rpcModel.m_entries.Count)
            {
                var entry = rpcModel.m_entries[matchIndex++];
                _serverProperty.m_logs.Add(entry);
                serverEventMaster.CallOnAddCommand(entry.m_command, _serverProperty.m_logs.Count);
            }
        }

        // If leaderCommit > commitIndex, update commitIndex
        if (rpcModel.m_leaderCommit > _serverProperty.m_commitIndex)
        {
            _serverProperty.m_commitIndex = Mathf.Min(rpcModel.m_leaderCommit, _serverProperty.m_logs.Count);
        }

        _rpcSender.SendAppendEntriesRPCReturn(_serverProperty.m_currentTerm, true, _serverProperty.m_serverId, leader.transform);
    }

    private void ProcessAppendEntriesReturn(RaftAppendEntriesReturns rpcModel)
    {
        if (_serverStateController.m_stateType != RaftStateType.Leader) return;

        // If successful: update nextIndex and matchIndex for follower
        if (rpcModel.m_success)
        {
            _serverProperty.m_nextIndex[rpcModel.m_followerId - 1] = _serverProperty.m_lastReplicateIndex[rpcModel.m_followerId - 1] + 1;
            _serverProperty.m_matchIndex[rpcModel.m_followerId - 1] = _serverProperty.m_lastReplicateIndex[rpcModel.m_followerId - 1];
        }
        // AppendEntries fails because of log inconsistency, decrement the nextIndex
        else
        {
            _serverProperty.m_nextIndex[rpcModel.m_followerId - 1]--;
        }
    }

    private void ProcessRequestVote(RaftRequestVoteArgus rpcModel)
    {
        var candidate = RaftServerManager.Instance.GetServer(rpcModel.m_candidateId);
        if (candidate == null) return;

        // If a follower receive RPC from other, it will reset the time
        if (_serverStateController.m_stateType == RaftStateType.Follower)
        {
            _serverStateController.m_currentState.InitializeState(_serverProperty);
        }

        // If candidate's term > currentTerm, reply false
        if (rpcModel.m_term > _serverProperty.m_currentTerm)
        {
            _rpcSender.SendRequestVoteRPCReturn(_serverProperty.m_currentTerm, false, candidate.transform);
            return;
        }

        bool voteGranted = false;

        // If votedFor is null or candidateId, and candidate’s log is at least as up - to - date as receiver’s log, grant vote
        if ((_serverProperty.m_votedFor <= 0) || (_serverProperty.m_votedFor == rpcModel.m_candidateId))
        {
            int lastIndex = _serverProperty.m_logs.Count;
            int lastTerm = lastIndex == 0 ? 0 : _serverProperty.m_logs[lastIndex - 1].m_term;

            if (CompareLogPriority(rpcModel.m_lastLogIndex, rpcModel.m_lastLogTerm, lastIndex, lastTerm) >= 0)
            {
                voteGranted = true;
            }
        }

        // Chage server voteFor property if vote granted
        if (voteGranted)
        {
            _serverProperty.m_votedFor = rpcModel.m_candidateId;
        }

        // Send the returns
        _rpcSender.SendRequestVoteRPCReturn(_serverProperty.m_currentTerm, voteGranted, candidate.transform);
    }


    private void ProcessRequestVoteReturn(RaftRequestVoteReturns rpcModel)
    {
        if (_serverStateController.m_stateType == RaftStateType.Candidate)
        {
            var candidate = (RaftCandidateState)_serverStateController.m_currentState;

            if (rpcModel.m_voteGranted)
            {
                int voteNumber = 0;
                while (candidate.m_voteResult[voteNumber]) voteNumber++;
                candidate.m_voteResult[voteNumber] = true;

                // If vote from majority of servers, become leader
                if (2 * (voteNumber + 1) > candidate.m_voteResult.Length)
                {
                    _serverStateController.m_currentState = _serverStateController.GetState("Leader State");
                    _serverStateController.m_currentState.InitializeState(_serverProperty);
                }
            }
        }
    }


    // Raft determines which of two logs is more up-to-date by comparing the index and term of the last entries in the logs
    // If the logs have last entries with different terms, then the log with the later term is more up-to-date.
    // If the logs end with the same term, then whichever log is longer is more up-to-date.
    /// <summary>
    /// Compare which log is more up-to-date
    /// </summary> 
    /// <returns>Return positive number if log1 is more up-to-date than log2. Return zero if their priority are same</returns>
    private int CompareLogPriority(int lastLogIndex1, int lastLogTerm1, int lastLogIndex2, int lastLogTerm2)
    {
        if (lastLogTerm1 != lastLogTerm2)
        {
            return lastLogTerm1 - lastLogTerm2;
        }
        else
        {
            return lastLogIndex1 - lastLogIndex2;
        }
    }
}
