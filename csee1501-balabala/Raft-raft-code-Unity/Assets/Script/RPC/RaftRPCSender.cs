using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftRPCSender : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject m_appendEntriesArguPrefab;
    public GameObject m_appendEntriesRetuPrefab;
    public GameObject m_requestVoteArguPrefab;
    public GameObject m_requestVoteRetuPrefab;

    [Header("Icons")]
    public Sprite m_normalSendIcon;
    public Sprite m_voteTrueSendIcon;
    public Sprite m_voteFalseSendIcon;


    /// <summary>
    /// Invoked by leader to append entries to a certain target
    /// </summary>
    /// <param name="term">Leader's term</param>
    /// <param name="leaderId">Leader's id. So follower can redirect clients</param>
    /// <param name="prevLogIndex">Index of log entry immediately preceding new ones</param>
    /// <param name="prevLogTerm">Term of prevLogIndex</param>
    /// <param name="entries">Log entries to store (empty for heartbeat; may send more than one entry for efficiency)</param>
    /// <param name="leaderCommit">Leader's committed index</param>
    public void SendAppendEntriesRPCArgu(int term, int leaderId, int prevLogIndex, int prevLogTerm, List<RaftEntry> entries, int leaderCommit, Transform target)
    {
        GameObject appendEntriesGo = Instantiate(m_appendEntriesArguPrefab, transform.position, Quaternion.identity);

        // Set append entries arguments
        var argus = appendEntriesGo.GetComponent<RaftAppendEntriesArgus>();
        argus.m_rpcType = RaftRPCType.AppendEntriesArgu;
        argus.m_target = target.transform;
        argus.m_term = term;
        argus.m_leaderId = leaderId;
        argus.m_prevLogIndex = prevLogIndex;
        argus.m_prevLogTerm = prevLogTerm;
        argus.m_entries = entries;
        argus.m_leaderCommit = leaderCommit;

        // Init move script
        var moveToward = appendEntriesGo.GetComponent<MoveToward>();
        moveToward.m_target = target.transform;
        moveToward.enabled = true;

        // Set sprite
        appendEntriesGo.GetComponent<SpriteRenderer>().sprite = m_normalSendIcon;
    }


    /// <summary>
    /// Send AppendEntries returns to leader
    /// </summary>
    /// <param name="term">sender's currentTerm, for leader to update itself</param>
    /// <param name="success">true if follower contained entry matching leader's prevLogIndex and prevLogTerm</param>
    public void SendAppendEntriesRPCReturn(int term, bool success, int followerId, Transform leader)
    {
        var appendEntriesGo = Instantiate(m_appendEntriesRetuPrefab, transform.position, Quaternion.identity);

        // Set returns property
        var appendReturn = appendEntriesGo.GetComponent<RaftAppendEntriesReturns>();
        appendReturn.m_rpcType = RaftRPCType.AppendEntriesReturn;
        appendReturn.m_target = leader;
        appendReturn.m_term = term;
        appendReturn.m_success = success;
        appendReturn.m_followerId = followerId;

        // Set move script
        var moveToward = appendEntriesGo.GetComponent<MoveToward>();
        moveToward.m_target = leader;
        moveToward.enabled = true;

        // Set sprite
        appendEntriesGo.GetComponent<SpriteRenderer>().sprite = success ? m_voteTrueSendIcon : m_voteFalseSendIcon;
    }

    /// <summary>
    /// Invoked by candidates to gather vote
    /// </summary>
    /// <param name="term">Candidate's term</param>
    /// <param name="candidateId">Id of the candidate which request vote</param>
    /// <param name="lastLogIndex">Index of candidatas last log entry</param>
    /// <param name="lastLogTerm">Term of candidata last log entry</param>
    /// <returns></returns>
    public void SendRequestVoteRPCArgu(int term, int candidateId, int lastLogIndex, int lastLogTerm)
    {
        foreach (var server in RaftServerManager.Instance.m_servers)
        {
            if (server.m_serverId != candidateId)
            {
                GameObject requestVoteGo = Instantiate(m_requestVoteArguPrefab, transform.position, Quaternion.identity);

                // Set request vote arguments
                var argus = requestVoteGo.GetComponent<RaftRequestVoteArgus>();
                argus.m_rpcType = RaftRPCType.RequestVoteArgu;
                argus.m_target = server.transform;
                argus.m_term = term;
                argus.m_candidateId = candidateId;
                argus.m_lastLogIndex = lastLogIndex;
                argus.m_lastLogTerm = lastLogTerm;

                // Init move script
                var moveToward = requestVoteGo.GetComponent<MoveToward>();
                moveToward.m_target = server.transform;
                moveToward.enabled = true;

                // Set sprite
                requestVoteGo.GetComponent<SpriteRenderer>().sprite = m_normalSendIcon;
            }
        }
    }


    /// <summary>
    /// Inoved by receiver to return vote RPC
    /// </summary>
    /// <param name="term">Current term, for candidate to update itself</param>
    /// <param name="voteGranted">True means candidate received vote</param>
    /// <param name="target">Candidate</param>
    public void SendRequestVoteRPCReturn(int term, bool voteGranted, Transform candidate)
    {
        var requestReturnGo = Instantiate(m_requestVoteRetuPrefab, transform.position, Quaternion.identity);

        // Set return properties
        var requestReturn = requestReturnGo.GetComponent<RaftRequestVoteReturns>();
        requestReturn.m_target = candidate;
        requestReturn.m_rpcType = RaftRPCType.RequestVoteReturn;
        requestReturn.m_term = term;
        requestReturn.m_voteGranted = voteGranted;

        // Set move script
        var moveToward = requestReturnGo.GetComponent<MoveToward>();
        moveToward.m_target = candidate;
        moveToward.enabled = true;

        // Set sprite
        requestReturnGo.GetComponent<SpriteRenderer>().sprite = voteGranted ? m_voteTrueSendIcon : m_voteFalseSendIcon;
    }

}
