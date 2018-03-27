using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftClient : RaftSingletonMonoBehavior<RaftClient>
{
    public Queue<char> m_commandCache;

    public List<char> m_historicCommand;

    /// <summary>
    /// Called when client add command.
    /// Parameter: char? -- command; int -- added command index
    /// </summary>
    public System.Action<char, int> OnAddCommand;

    /// <summary>
    /// Called when server get command.
    /// Parameter: char? -- command
    /// </summary>
    public System.Action<char> OnGetCommand;    

    /// <summary>
    /// Add a command into command cache
    /// </summary>
    public void AddCommand(char command)
    {
        if (m_commandCache == null)
        {
            m_commandCache = new Queue<char>();
        }
        m_commandCache.Enqueue(command);

        if (m_historicCommand == null)
        {
            m_historicCommand = new List<char>();
        }
        m_historicCommand.Add(command);

        if (OnAddCommand != null)
        {
            OnAddCommand(command, m_historicCommand.Count);
        }
    }


    /// <summary>
    /// Get all command from command cache. Return null if there is no command.
    /// </summary>
    public List<char> GetCommand()
    {
        List<char> commands = new List<char>();

        while (m_commandCache != null && m_commandCache.Count > 0)
        {
            char command = m_commandCache.Dequeue();
            commands.Add(command);

            if (OnGetCommand != null)
            {
                OnGetCommand(command);
            }
        }

        if (commands.Count != 0)
        {
            return commands;
        }
        else
        {
            return null;
        }
    }

}
