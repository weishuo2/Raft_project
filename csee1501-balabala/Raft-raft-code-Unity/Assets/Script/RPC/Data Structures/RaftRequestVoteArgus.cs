using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftRequestVoteArgus : RaftBaseRPCModel
{                      
    /// <summary>
    /// Id of the candidate which request vote
    /// </summary>
    public int m_candidateId;

    /// <summary>
    /// Index of candidatas last log entry
    /// </summary>
    public int m_lastLogIndex;

    /// <summary>
    /// Term of candidata last log entry
    /// </summary>
    public int m_lastLogTerm;  
}
