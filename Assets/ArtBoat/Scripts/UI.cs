using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    public HighScores highScorePanel;
    public TimePanel timePanel;

    //this probably shouldn't be here? w.e
    private float time = 0f;
    private bool timeStart = false;

    private void Awake()
    {
        timePanel.gameObject.SetActive(false);
    }

    public void ShowHighScores()
    {
        timeStart = false;
        timePanel.gameObject.SetActive(false);

        highScorePanel.AddScoreAndShow(time);
    }

    public void StartTime()
    {
        timePanel.gameObject.SetActive(true);
        timeStart = true;
    }

    private void Update()
    {
        if(timeStart)
        {
            time += Time.deltaTime;
            System.TimeSpan t = System.TimeSpan.FromSeconds(time);
            timePanel.timeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
    }
}
