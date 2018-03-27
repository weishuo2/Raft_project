using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftBaseState : MonoBehaviour
{
    [Header("References")]
    public RaftStateController m_stateController;
    public RaftServerEventMaster m_eventMaster;

    public virtual void InitializeState(RaftServerProperty serverProperty) { }

    public virtual void UpdateState(RaftServerProperty serverProperty)
    {
        // If commitIndex > lastApplied, apply log[lastApplied] to state machine
        if (serverProperty.m_commitIndex > serverProperty.m_lastApplied)
        {
            int newLastApplied = serverProperty.m_commitIndex;
            for (int i = serverProperty.m_lastApplied; i < newLastApplied; i++)
            {
                m_eventMaster.CallOnApplyCommand(serverProperty.m_logs[i].m_command, serverProperty.m_logs[i].m_term, i + 1);
            }
            serverProperty.m_lastApplied = newLastApplied;
        }
    }
}
