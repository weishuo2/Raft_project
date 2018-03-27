using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftAppendEntriesReturns : RaftBaseRPCModel
{                         
    /// <summary>
    /// True if follower contain log entry matching prevLogIndex and prevLogTerm
    /// </summary>
    public bool m_success;

    public int m_followerId;
}
