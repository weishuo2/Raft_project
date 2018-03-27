using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowServerInfoButtonController : MonoBehaviour
{
    public GameObject m_serverInfoUI;

    public void ShowServerInfo(RaftServerProperty serverProperty)
    {
        m_serverInfoUI.SetActive(true);

        m_serverInfoUI.GetComponent<ServerInfoUIController>().SetValue(serverProperty);
    }
}
