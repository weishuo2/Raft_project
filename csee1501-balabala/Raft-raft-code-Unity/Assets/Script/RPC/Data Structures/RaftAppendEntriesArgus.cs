using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftAppendEntriesArgus : RaftBaseRPCModel
{                                  
    /// <summary>
    /// Leader's id. So follower can redirect clients
    /// </summary>
    public int m_leaderId;

    /// <summary>
    /// Index of log entry immediately preceding new ones
    /// </summary>
    public int m_prevLogIndex;

    /// <summary>
    /// Term of prevLogIndex
    /// </summary>
    public int m_prevLogTerm;

    /// <summary>
    /// Log entries to store (empty for heartbeat; may send more than one entry for efficiency)
    /// </summary>
    public List<RaftEntry> m_entries;

    /// <summary>
    /// Leader's committed index
    /// </summary>
    public int m_leaderCommit;
}
