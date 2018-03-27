using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftLeaderState : RaftBaseState
{
    [Header("Candidate Relative Property")]
    public float m_heartbeatDue = 0.050f;

    private float _heartbeatTimer;

    public override void InitializeState(RaftServerProperty serverProperty)
    {
        base.InitializeState(serverProperty);

        m_stateController.m_stateType = RaftStateType.Leader;

        int serverNumber = RaftServerManager.Instance.m_totalServerCount;

        // Initialize leader's nextIndex, matchIndex and lastReplicateIndex
        serverProperty.m_nextIndex = new List<int>(serverNumber);
        serverProperty.m_matchIndex = new List<int>(serverNumber);
        serverProperty.m_lastReplicateIndex = new List<int>(serverNumber);
        for (int i = 0; i < serverNumber; i++)
        {
            serverProperty.m_nextIndex.Add(serverProperty.m_logs.Count + 1);
            serverProperty.m_matchIndex.Add(0);
            serverProperty.m_lastReplicateIndex.Add(0);
        }

        // Send initial empty AppendEntries RPC
        SendAppendEntries(serverProperty, true);

        _heartbeatTimer = 0f;
    }

    public override void UpdateState(RaftServerProperty serverProperty)
    {
        base.UpdateState(serverProperty);

        // If command received from client, append entry to local log
        List<char> commands = RaftClient.Instance.GetCommand();
        if ((commands != null) && (commands.Count != 0))
        {
            var serverEventMaster = serverProperty.GetComponent<RaftServerEventMaster>();
            foreach (var command in commands)
            {
                serverProperty.m_logs.Add(new RaftEntry(command, serverProperty.m_currentTerm));
                serverProperty.m_matchIndex[serverProperty.m_serverId - 1] = serverProperty.m_logs.Count;
                serverEventMaster.CallOnAddCommand(command, serverProperty.m_logs.Count);                
            }
        }

        // If exist N > commitIndex, and majority of matchIndex[i] >= N, then log[N].term = currentTerm, set commitIndex = N
        for (int n = serverProperty.m_commitIndex + 1; n <= serverProperty.m_logs.Count; n++)
        {
            int commitNumber = 0;
            int serverNumber = RaftServerManager.Instance.m_totalServerCount;
            for (int i = 0; i < serverNumber; i++)
            {
                if (serverProperty.m_matchIndex[i] >= n) commitNumber++;
            }

            if (commitNumber * 2 > serverNumber)
            {
                serverProperty.m_logs[n - 1].UpdateTerm(serverProperty.m_currentTerm);
                serverProperty.m_commitIndex = n;
            }
        }


        _heartbeatTimer += RaftTime.Instance.DeltTime;

        if (_heartbeatTimer >= m_heartbeatDue)
        {
            SendAppendEntries(serverProperty, false);

            _heartbeatTimer = 0;
        }
    }


    /// <summary>
    /// Send AppendEntries RPC to all servers
    /// </summary>
    /// <param name="empty">Send empty append entries?</param>
    private void SendAppendEntries(RaftServerProperty serverProperty, bool empty)
    {
        var sender = serverProperty.GetComponent<RaftRPCSender>();

        foreach (var server in RaftServerManager.Instance.m_servers)
        {
            if (server.m_serverId != serverProperty.m_serverId)
            {
                List<RaftEntry> entries = new List<RaftEntry>();

                // If last log index >= nextIndex for a follower : send AppendEntries RPC with log entries starting at nextIndex
                int nextIndex = serverProperty.m_nextIndex[server.m_serverId - 1];
                if (serverProperty.m_logs.Count >= nextIndex)
                {
                    for (int i = nextIndex; i <= serverProperty.m_logs.Count; i++)
                    {
                        entries.Add(serverProperty.m_logs[i - 1]);
                    }
                }
                if (empty) entries = null;

                int prevLogIndex = nextIndex - 1;
                int prevLogTerm = prevLogIndex == 0 ? 0 : serverProperty.m_logs[prevLogIndex - 1].m_term;

                sender.SendAppendEntriesRPCArgu(serverProperty.m_currentTerm,
                                serverProperty.m_serverId,
                                prevLogIndex, prevLogTerm, entries,
                                serverProperty.m_commitIndex,
                                server.transform);

                // Update lastReplicateIndex upon send the entries 
                if (entries != null && entries.Count > 0)
                {
                    serverProperty.m_lastReplicateIndex[server.m_serverId - 1] = serverProperty.m_logs.Count;
                }
            }

        }
    }
}
