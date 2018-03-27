using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftSceneRestart : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DontDestroyOnLoad(RaftServerManager.Instance.gameObject);
            DontDestroyOnLoad(RaftClient.Instance.gameObject);

            if (RaftClient.Instance.m_commandCache != null)
                RaftClient.Instance.m_commandCache.Clear();
            if (RaftClient.Instance.m_historicCommand != null)
                RaftClient.Instance.m_historicCommand.Clear();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("main");
        }
    }
}
