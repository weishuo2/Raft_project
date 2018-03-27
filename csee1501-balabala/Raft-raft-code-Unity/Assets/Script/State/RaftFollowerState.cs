using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftFollowerState : RaftBaseState
{
    [Space]
    [Header("Follower Relative Property")]
    /// <summary>
    /// If a follower recerive no communication over the m_electionTimeout, 
    /// it will assumes there is no viable leader and begins an election to chooses a leader.
    /// </summary>
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

    public override void InitializeState(RaftServerProperty serverProperty)
    {
        base.InitializeState(serverProperty);

        m_stateController.m_stateType = RaftStateType.Follower;

        // Set timeout
        m_electionTimeout = Random.Range(m_minElectionTimeout, m_maxElectionTimeout);
        m_electionTimer = 0;
    }

    public override void UpdateState(RaftServerProperty serverProperty)
    {
        base.UpdateState(serverProperty);

        m_electionTimer += RaftTime.Instance.DeltTime;

        // If a follower recerive no communication over the m_electionTimeout, 
        // it will assumes there is no viable leader and begins an election to chooses a leader.
        if (m_electionTimer >= m_electionTimeout)
        {
            // Transition to candidate state
            m_stateController.m_currentState = m_stateController.GetState("Candidate State");
            m_stateController.m_currentState.InitializeState(serverProperty);
        }    
    }


}
