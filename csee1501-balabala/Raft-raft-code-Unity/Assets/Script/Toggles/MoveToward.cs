using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToward : MonoBehaviour
{
    public Transform m_target;

    // Boardcast time is the average time it takes a server to send RPCs to other server
    // Raft's RPC typically require the recipient to persist information to stable storage, so the boardcast time may range from 2ms to 20ms
    public float m_maxBoardcastTime = 0.002f;
    public float m_minBoardcastTime = 0.02f;

    private float _speed;
    private Vector3 _moveDir;

    private void Start()
    {
        float boardcastTime = Random.Range(m_minBoardcastTime, m_maxBoardcastTime);
        _speed = (m_target.position - transform.position).magnitude / boardcastTime;
        _moveDir = (m_target.position - transform.position).normalized;
    }

    private void Update()
    {
        if (m_target != null)
        {
            float moveDistance = _speed * RaftTime.Instance.DeltTime;
            float distance = (m_target.position - transform.position).magnitude;

            if (moveDistance >= distance)
            {
                transform.position = m_target.position;
                enabled = false;
                return;
            }
            else
            {
                transform.position += _moveDir * moveDistance;
            }
        }
    }
}
