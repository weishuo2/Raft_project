/// <summary>
/// Each entry contains a command and term when entry was received by leader
/// </summary>
[System.Serializable]
public struct RaftEntry
{
    public char m_command;

    /// <summary>
    /// When entry was received by leader.
    /// -1 if this entry not applied.
    /// </summary>
    public int m_term;

    public RaftEntry(char command, int term)
    {
        m_command = command;
        m_term = term;
    }

    public void UpdateTerm(int term)
    {
        m_term = term;
    }

    public void UpdateCommand(char command)
    {
        m_command = command;
    }
}