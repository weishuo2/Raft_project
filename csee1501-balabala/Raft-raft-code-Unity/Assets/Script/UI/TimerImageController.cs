using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimerImageController : MonoBehaviour
{
    public Image m_timerImage;

    public Color m_timerColor;
    public Color m_leaderColor;

    public RaftStateController m_stateController;

    private void Update()
    {
        if(m_stateController.m_stateType == RaftStateType.Leader)
        {
            m_timerImage.color = m_leaderColor;
            m_timerImage.fillAmount = 1;
        }
        else if(m_stateController.m_stateType == RaftStateType.Follower)
        {
            m_timerImage.color = m_timerColor;
            var follower = (RaftFollowerState)m_stateController.m_currentState;
            m_timerImage.fillAmount = 1 - follower.m_electionTimer / follower.m_electionTimeout;
        }
        else if(m_stateController.m_stateType == RaftStateType.Candidate)
        {
            m_timerImage.color = m_timerColor;
            var candidate = (RaftCandidateState)m_stateController.m_currentState;
            m_timerImage.fillAmount = 1 - candidate.m_electionTimer / candidate.m_electionTimeout;
        } 
    }
}
