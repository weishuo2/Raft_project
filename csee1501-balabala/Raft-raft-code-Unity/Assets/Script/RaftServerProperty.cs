using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftServerProperty : MonoBehaviour
{
    // ***************  Persistent *********************
    // Update on stable storage before responding to RPCs
    /// <summary>
    /// Id that can identify a server
    /// </summary>
    [Header("Persistant Properties")]
    public int m_serverId;

    /// <summary>
    /// Last term this server has ever seen
    /// Initialize to zero on first boot and increases monotonically
    /// </summary>
    public int m_currentTerm;

    /// <summary>
    /// Candidate Id that received vote in current term.(or null if none)
    /// </summary>
    public int m_votedFor;

    /// <summary>
    /// Log entries. Each entry contains command for state machine, and term when entry was received by leader. (first index is one)
    /// </summary>
    public List<RaftEntry> m_logs;


    // ***************** Volatile On All Servers ********************************
    /// <summary>
    /// Index of highest log entry known to be committed.
    /// Initialize to zero, increase monotonically
    /// </summary>
    [Header("Volatile Property")]
    public int m_commitIndex;

    /// <summary>
    /// Index of highest log entry apply to server
    /// Initialize to zero, increase monotonically
    /// </summary>
    public int m_lastApplied;


    // ****************** Volatile On Leaders ***********************************
    // Reinitialized after election
    /// <summary>
    /// For each server, index of next log entry send to that server
    /// Initialized to leader last log index + 1
    /// </summary>
    [Header("Volatile Property for Leader")]
    public List<int> m_nextIndex;

    /// <summary>
    /// For each server, index of highest log entry known to be replicated on server
    /// Initialize to 0, increase monotonically
    /// </summary>
    public List<int> m_matchIndex;

    /// <summary>
    /// For each server, index of highest log entry kown to be send to server (not means server has applied it)
    /// </summary>
    public List<int> m_lastReplicateIndex;


    private void OnEnable()
    {
        GetComponent<RaftServerEventMaster>().OnChangeTerm += UpdateProperties;


        RaftServerManager.Instance.m_servers.Add(this);
    }

    private void OnDisable()
    {
        GetComponent<RaftServerEventMaster>().OnChangeTerm -= UpdateProperties;

        if (RaftServerManager.Instance.m_servers != null)
        {
            RaftServerManager.Instance.m_servers.Remove(this);
        }
    }

    private void UpdateProperties(int currentTerm)
    {
        m_votedFor = 0;
    }
}
