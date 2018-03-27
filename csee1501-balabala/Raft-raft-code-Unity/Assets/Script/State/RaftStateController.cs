using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftStateController : MonoBehaviour
{
    public RaftStateType m_stateType;

    public RaftBaseState m_currentState;

    public List<RaftBaseState> m_totalState;

    private RaftServerProperty _serverProperty;

    private void Awake()
    {
        _serverProperty = GetComponent<RaftServerProperty>();

        foreach (var state in m_totalState)
        {
            state.m_stateController = this;
            state.m_eventMaster = GetComponent<RaftServerEventMaster>();
        }
    }

    private void OnEnable()
    {
        // When the server start up, it begin as follower
        m_stateType = RaftStateType.Follower;
        m_currentState = GetState("Follower State");
        m_currentState.InitializeState(_serverProperty);
    }

    private void Update()
    {
        m_currentState.UpdateState(_serverProperty);
    }

    /// <summary>
    /// Get a state by its gameobject name, return null if not find
    /// </summary>
    public RaftBaseState GetState(string stateName)
    {
        foreach (var state in m_totalState)
        {
            if (state.gameObject.name == stateName)
            {
                return state;
            }
        }

        return null;
    }



}
