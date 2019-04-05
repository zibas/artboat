using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//please don't judge me for the contents of this script file -ckm
public class HighScores : MonoBehaviour {
    public Transform highScoreParent;
    public Text yourScoreText;

    int highScoreCount = 10;
    Text[] highScoreTexts;

    public Color YourScoreColor = Color.white;
    public int YourScoreFontSizeIncrease = 10;

    Color defaultFontColor;
    int defaultFontSize;

	void Awake() {
        highScoreTexts = new Text[highScoreParent.childCount];
        for(int i = 0; i < highScoreParent.childCount; i++)
        {
            highScoreTexts[i] = highScoreParent.GetChild(i).GetComponent<Text>();
        }

        defaultFontSize = highScoreTexts[0].fontSize;
        defaultFontColor = highScoreTexts[0].color;

        gameObject.SetActive(false);
	}

    public void AddScoreAndShow(float newScore)
    {
        //load in the previous scores
        List<float> scores = new List<float>();
        for (int i = 0; i < highScoreCount; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i))
            {
                scores.Add(PlayerPrefs.GetFloat("HighScore" + i));
            }
        }
        
        //add our new score, sort, then get the top 10
        scores.Add(newScore);
        scores = scores.OrderByDescending(s => s).Reverse().Take(highScoreCount).ToList();

        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            if(scores.Count > i)
            {
                //record the score
                PlayerPrefs.SetFloat("HighScore" + i, scores[i]);

                //then make timespans out of the floats so we can print em easier and display em
                System.TimeSpan time = System.TimeSpan.FromSeconds(scores[i]);
                highScoreTexts[i].text = string.Format("{0:D2} : {1:D2}", time.Minutes, time.Seconds);

                if(scores[i] == newScore)
                {
                    highScoreTexts[i].fontSize = defaultFontSize + YourScoreFontSizeIncrease;
                    highScoreTexts[i].color = YourScoreColor;
                }
                else
                {
                    highScoreTexts[i].fontSize = defaultFontSize;
                    highScoreTexts[i].color = defaultFontColor;
                }
            } else
            {
                highScoreTexts[i].text = "";
            }
        }

        System.TimeSpan newTime = System.TimeSpan.FromSeconds(newScore);
        yourScoreText.text = string.Format("{0:D2} : {1:D2}", newTime.Minutes, newTime.Seconds);

        gameObject.SetActive(true);
    }

    public void ResetScores()
    {
        for(int i = 0; i < highScoreCount; i++)
        {
            if(PlayerPrefs.HasKey("HighScore" + i))
            {
                PlayerPrefs.DeleteKey("HighScore" + i);
            }
        }
    }

    public void Reload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


    public void FakeHighScores()
    {
        PlayerPrefs.SetFloat("HighScore0", 30f);
        PlayerPrefs.SetFloat("HighScore1", 40f);
        PlayerPrefs.SetFloat("HighScore2", 50f);
        PlayerPrefs.SetFloat("HighScore3", 60f);
        PlayerPrefs.SetFloat("HighScore4", 120f);
        PlayerPrefs.SetFloat("HighScore5", 180f);
        PlayerPrefs.SetFloat("HighScore6", 185f);
        PlayerPrefs.SetFloat("HighScore7", 190f);
        PlayerPrefs.SetFloat("HighScore8", 200f);
        PlayerPrefs.SetFloat("HighScore9", 200f);
    }
}
