using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommitButtonController : MonoBehaviour
{
    public TMPro.TMP_InputField m_inputField;

    public void Commit()
    {
        if (!string.IsNullOrEmpty(m_inputField.text))
        {
            RaftClient.Instance.AddCommand(m_inputField.text[0]);
            m_inputField.text = string.Empty;
        }
    }

}
