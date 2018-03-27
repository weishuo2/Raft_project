using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaftServerEventMaster : MonoBehaviour
{
    /// <summary>
    /// Called when server change its term
    /// </summary>
    public Action<int> OnChangeTerm;

    /// <summary>
    /// Called when server apply a command.
    /// Parameter 1 : command.
    /// Parameter 2 : log term.
    /// Parameter 3 : log index.
    /// </summary>
    public Action<char, int, int> OnApplyCommand;

    /// <summary>
    /// Called when server add a command.
    /// Parameter 1 : command.
    /// Parameter 2 : log term.
    /// </summary>
    public Action<char, int> OnAddCommand;

    public void CallOnChangeTerm(int currentTerm)
    {
        if(OnChangeTerm != null)
        {
            OnChangeTerm(currentTerm);
        }
    }

    public void CallOnApplyCommand(char command, int logTerm, int logIndex)
    {
        if(OnApplyCommand != null)
        {
            OnApplyCommand(command, logTerm, logIndex);
        }
    }

    public void CallOnAddCommand(char command, int logIndex)
    {
        if(OnAddCommand != null)
        {
            OnAddCommand(command, logIndex);
        }
    }
}
