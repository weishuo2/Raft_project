using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteForTextController : MonoBehaviour
{
    public RaftServerProperty m_serverProperty;

    public TMPro.TextMeshProUGUI m_voteForText;

    private void Update()
    {
        if(m_serverProperty.m_votedFor <= 0)
        {
            m_voteForText.text = "VoteFor : null";
        }
        else
        {
            m_voteForText.text = string.Format("VoteFor : {0}", m_serverProperty.m_votedFor);
        }
    }

}
