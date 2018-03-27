using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerIdTextController : MonoBehaviour
{
    public RaftServerProperty m_serverProperty;

    public TMPro.TextMeshProUGUI m_serverIdText;

    private void OnEnable()
    {
        m_serverIdText.text = string.Format("S{0}", m_serverProperty.m_serverId);    
    }
}
