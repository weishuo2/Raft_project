using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTermTextController : MonoBehaviour
{
    public RaftServerEventMaster m_serverEventMaster;

    public TMPro.TextMeshProUGUI m_curTermText;

    private void OnEnable()
    {
        // Initialize current text
        m_curTermText.text = m_serverEventMaster.GetComponent<RaftServerProperty>().m_currentTerm.ToString();

        m_serverEventMaster.OnChangeTerm += ApplyCurrentTermUI;    
    }

    private void OnDisable()
    {
        m_serverEventMaster.OnChangeTerm -= ApplyCurrentTermUI;
    }


    private void ApplyCurrentTermUI(int currentTerm)
    {
        m_curTermText.text = currentTerm.ToString();
    }
}
