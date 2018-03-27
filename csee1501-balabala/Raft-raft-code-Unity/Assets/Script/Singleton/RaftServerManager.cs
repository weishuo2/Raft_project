using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftServerManager : RaftSingletonMonoBehavior<RaftServerManager>
{
    /// <summary>
    /// Total server in current enviroment, include active and deactive server
    /// It show bu set right before use raft algorithm
    /// </summary>
    public int m_totalServerCount;

    /// <summary>
    /// Total server in current enviroment. Only include active server
    /// </summary>
    public List<RaftServerProperty> m_servers;

    /// <summary>
    /// Get a server by its id
    /// </summary>           
    /// <returns>Return server property or null if not find</returns>
    public RaftServerProperty GetServer(int serverId)
    {
        foreach(var server in m_servers)
        {
            if(server.m_serverId == serverId)
            {
                return server;
            }
        }

        return null;
    }
}
