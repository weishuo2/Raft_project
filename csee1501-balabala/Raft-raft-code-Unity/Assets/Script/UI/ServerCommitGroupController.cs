using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCommitGroupController : MonoBehaviour
{
    public RaftServerEventMaster m_serverEventMaster;

    public Transform m_commandParent;

    public Transform m_edgeParent;

    private void OnEnable()
    {
        m_serverEventMaster.OnAddCommand += ApplyServerAddCommandUI;
        m_serverEventMaster.OnApplyCommand += ApplyServerCommitUI;
    }

    private void OnDisable()
    {
        m_serverEventMaster.OnAddCommand -= ApplyServerAddCommandUI;
        m_serverEventMaster.OnApplyCommand -= ApplyServerCommitUI;
    }

    private void ApplyServerCommitUI(char command, int logTerm, int logIndex)
    {
        logIndex--;

        var commandTex = m_commandParent.GetChild(logIndex).GetComponent<TMPro.TextMeshProUGUI>();
        commandTex.text = string.Format("{0}:{1}", logTerm, command);
        commandTex.color = Color.black;

        var edgeImange = m_edgeParent.GetChild(logIndex).GetComponent<UnityEngine.UI.Image>();
        edgeImange.color = Color.black;
    }

    private void ApplyServerAddCommandUI(char command, int logIndex)
    {
        logIndex--;

        var commandTex = m_commandParent.GetChild(logIndex).GetComponent<TMPro.TextMeshProUGUI>();
        commandTex.text = string.Format(" :{0}", command);
        commandTex.color = Color.gray;

        var edgeImange = m_edgeParent.GetChild(logIndex).GetComponent<UnityEngine.UI.Image>();
        edgeImange.color = Color.gray;
    }
}


