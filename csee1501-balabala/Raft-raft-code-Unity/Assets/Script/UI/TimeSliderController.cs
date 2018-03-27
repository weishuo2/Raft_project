using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSliderController : MonoBehaviour
{
    public Slider m_timeSlider;
    public TMPro.TextMeshProUGUI m_timeText;

    private void Update()
    {
        m_timeSlider.value = RaftTime.Instance.CurrentTime;
        m_timeText.text = RaftTime.Instance.CurrentTime.ToString("F3");
    }
}
