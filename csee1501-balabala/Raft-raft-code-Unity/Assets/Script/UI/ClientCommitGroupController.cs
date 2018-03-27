using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCommitGroupController : MonoBehaviour
{
    public Transform m_commandParent;

    public Transform m_edgeParent;

    private void OnEnable()
    {
        RaftClient.Instance.OnAddCommand += ApplyClientCommandUI;
    }

    private void OnDisable()
    {
        RaftClient.Instance.OnAddCommand -= ApplyClientCommandUI;
    }                                

    private void ApplyClientCommandUI(char command, int index)
    {
        index--;

        var commandText = m_commandParent.GetChild(index).GetComponent<TMPro.TextMeshProUGUI>();
        commandText.text = command.ToString();
        commandText.color = new Color(commandText.color.r, commandText.color.g, commandText.color.b, 1);

        var edgeImage = m_edgeParent.GetChild(index).GetComponent<UnityEngine.UI.Image>();
        edgeImage.color = new Color(edgeImage.color.r, edgeImage.color.g, edgeImage.color.b, 1);
    }
}
