using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftBaseRPCModel : MonoBehaviour
{
    public RaftRPCType m_rpcType;

    /// <summary>
    /// This RPC node will move to target
    /// </summary>
    public Transform m_target;

    public int m_term;
}
