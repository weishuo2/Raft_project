using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftCandidateState : RaftBaseState
{
    /// <summary>
    /// If a candidate timeout, it will start a new election by 
    /// increamenting its term and initiating another round of RequestVote RPCs 
    /// </summary>
    [Header("Candidate Relative Property")]
    [HideInInspector]
    public float m_electionTimeout;

    // Election timeout range.
    // Use randomized election timeout to ensure that split vote are rare and can be resolved quikly
    public float m_maxElectionTimeout = 0.15f;
    public float m_minElectionTimeout = 0.3f;

    /// <summary>
    /// Timer for electionTimeout
    /// </summary>
    [HideInInspector]
    public float m_electionTimer;

    /// <summary>
    /// All servers in the cluster vote result for candidata
    /// Key : server id. Value : vote result
    /// </summary>
    public bool[] m_voteResult;

    public override void InitializeState(RaftServerProperty serverProperty)
    {
        base.InitializeState(serverProperty);

        m_stateController.m_stateType = RaftStateType.Candidate;
        StartElection(serverProperty);
    }

    public override void UpdateState(RaftServerProperty serverProperty)
    {
        base.UpdateState(serverProperty);

        m_electionTimer += RaftTime.Instance.DeltTime;

        // If a candidate timeout, it will start a new election
        if (m_electionTimer >= m_electionTimeout)
        {
            StartElection(serverProperty);
        }
    }

    /// <summary>
    /// Start an election
    /// </summary>
    private void StartElection(RaftServerProperty serverProperty)
    {
        // Reset timeout
        m_electionTimeout = Random.Range(m_minElectionTimeout, m_maxElectionTimeout);
        m_electionTimer = 0;

        // Increment currentTerm
        serverProperty.m_currentTerm++;
        serverProperty.GetComponent<RaftServerEventMaster>().CallOnChangeTerm(serverProperty.m_currentTerm);

        // Vote for self
        serverProperty.m_votedFor = serverProperty.m_serverId;

        // Send RequestVote RPC to all other servers
        IssueRquestVotes(serverProperty);

        // Init vote granded dictionary
        InitiVoteGranded(serverProperty);
    }

    private void IssueRquestVotes(RaftServerProperty serverProperty)
    {
        int lastLogIndex = serverProperty.m_logs.Count;
        int lastLogTerm = (lastLogIndex == 0) ? 0 : serverProperty.m_logs[lastLogIndex - 1].m_term;

        var sender = serverProperty.GetComponent<RaftRPCSender>();
        sender.SendRequestVoteRPCArgu(serverProperty.m_currentTerm, serverProperty.m_serverId, lastLogIndex, lastLogTerm);
    }

    private void InitiVoteGranded(RaftServerProperty serverProperty)
    {
        m_voteResult = new bool[RaftServerManager.Instance.m_totalServerCount];

        for (int i = 0; i < m_voteResult.Length; i++) m_voteResult[i] = false;
        m_voteResult[0] = true;  // Vote for itself
    }

}
