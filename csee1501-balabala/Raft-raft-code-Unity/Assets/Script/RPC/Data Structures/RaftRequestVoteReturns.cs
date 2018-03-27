using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftRequestVoteReturns : RaftBaseRPCModel
{                        
    /// <summary>
    /// True means candidate received vote
    /// </summary>
    public bool m_voteGranted;
}
