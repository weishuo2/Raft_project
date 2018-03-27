using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ServerInfoUIController : MonoBehaviour
{
    [Header("Persistant Properties")]
    public TextMeshProUGUI m_serverIdText;

    public TextMeshProUGUI m_currentTermText;

    public TextMeshProUGUI m_voteForText;

    [Header("Volatile Properties")]
    public TextMeshProUGUI m_commitIndexText;

    public TextMeshProUGUI m_lastAppliedText;

    [Header("State Properties")]
    public TextMeshProUGUI m_currentStateText;

    public TextMeshProUGUI m_electionTimeoutText;

    public TextMeshProUGUI m_heartbeatDueText;

    public TextMeshProUGUI m_voteForResultText;

    private float _prevTimeScale;
    private RaftServerProperty _curServer;

    public void SetValue(RaftServerProperty serverProperty)
    {
        _prevTimeScale = RaftTime.Instance.TimeScale;
        _curServer = serverProperty;

        RaftTime.Instance.TimeScale = 0;

        RaftStateController stateController = serverProperty.GetComponent<RaftStateController>();

        m_serverIdText.text = string.Format("serverId : {0}", serverProperty.m_serverId);
        m_currentTermText.text = string.Format("currentTerm : {0}", serverProperty.m_currentTerm);
        m_voteForText.text = serverProperty.m_votedFor <= 0 ? "voteFor : null" : string.Format("voteFor : {0}", serverProperty.m_votedFor);

        m_commitIndexText.text = string.Format("commitIndex : {0}", serverProperty.m_commitIndex);
        m_lastAppliedText.text = string.Format("lastApplied : {0}", serverProperty.m_lastApplied);

        m_currentStateText.text = string.Format("currentState : {0}", stateController.m_stateType.ToString());

        m_voteForResultText.gameObject.SetActive(false);
        if (stateController.m_stateType == RaftStateType.Leader)
        {
            m_electionTimeoutText.gameObject.SetActive(false);
            m_heartbeatDueText.gameObject.SetActive(true);
            m_heartbeatDueText.text = string.Format("heartbeatDue : {0:F0}ms", ((RaftLeaderState)stateController.m_currentState).m_heartbeatDue * 1000);
            return;
        }

        m_electionTimeoutText.gameObject.SetActive(true);
        m_heartbeatDueText.gameObject.SetActive(false);
        if (stateController.m_stateType == RaftStateType.Follower)
        {
            m_electionTimeoutText.text = string.Format("electionTimeout : {0:F0}ms", ((RaftFollowerState)stateController.m_currentState).m_electionTimeout * 1000);
        }
        else
        {
            m_voteForResultText.gameObject.SetActive(true);

            var candidate = (RaftCandidateState)stateController.m_currentState;
            m_electionTimeoutText.text = string.Format("electionTimeout : {0:F0}ms", (candidate.m_electionTimeout * 1000));

            int voteNumber = 0;
            for (; voteNumber < candidate.m_voteResult.Length; voteNumber++)
            {
                if (!candidate.m_voteResult[voteNumber]) break;
            }
            m_voteForResultText.text = string.Format("voteNumber ; {0}", voteNumber);
        }
    }


    public void ExitServerInfoPanel()
    {
        RaftTime.Instance.TimeScale = _prevTimeScale;
        gameObject.SetActive(false);
    }

    public void StopServer()
    {
        if(!_curServer.enabled)
        {
            return;
        }

        var serverSprite = _curServer.GetComponent<SpriteRenderer>();
        var stateController = _curServer.GetComponent<RaftStateController>();
        var timeImage = _curServer.GetComponentInChildren<TimerImageController>();

        _curServer.enabled = false;
        serverSprite.color = Color.gray;
        stateController.enabled = false;
        timeImage.m_leaderColor = new Color(timeImage.m_leaderColor.r, timeImage.m_leaderColor.g, timeImage.m_leaderColor.b, 0);
        timeImage.m_timerColor= new Color(timeImage.m_timerColor.r, timeImage.m_timerColor.g, timeImage.m_timerColor.b, 0);
    }


    public void StartServer()
    {
        if(_curServer.enabled)
        {
            return;
        }

        var serverSprite = _curServer.GetComponent<SpriteRenderer>();
        var stateController = _curServer.GetComponent<RaftStateController>();
        var timeImage = _curServer.GetComponentInChildren<TimerImageController>();

        _curServer.enabled = true;
        serverSprite.color = Color.cyan;
        stateController.enabled = true;
        timeImage.m_leaderColor = new Color(timeImage.m_leaderColor.r, timeImage.m_leaderColor.g, timeImage.m_leaderColor.b, 1);
        timeImage.m_timerColor = new Color(timeImage.m_timerColor.r, timeImage.m_timerColor.g, timeImage.m_timerColor.b, 1);
    }
}
