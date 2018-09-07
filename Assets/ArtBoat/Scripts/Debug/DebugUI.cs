using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public GameObject Panel;

    UI ui;
    public WiimoteOars oars;

    public UnityEngine.UI.Text[] oarStateTexts;
    public UnityEngine.UI.Button[] oarStateButtons;

    private void Awake()
    {
        ui = FindObjectOfType<UI>();
        Panel.SetActive(false);
        for (int i = 0; i < oarStateButtons.Length; i++)
        {
            Debug.Log("add listener " + i);
            int x = i;
            oarStateButtons[i].onClick.AddListener(delegate {FlipOar(x); });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Panel.SetActive(!Panel.activeInHierarchy);
        }
        if (Panel.activeInHierarchy)
        {
            UpdateOars();
        }

    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ResetScores()
    {
        ui.highScorePanel.ResetScores();
    }

    public void FakeHighScores()
    {
        ui.highScorePanel.FakeHighScores();
    }

    private void UpdateOars()
    {
        if (oars != null)
        {
            int i = 0;
            foreach (var oar in oars.oars)
            {
                string s = oar.index + ": ";
                if (oar.inWater)
                {
                    s += "In Water ";
                }
                if (oar.isPaddling)
                {
                    s += " - " + oar.direction;
                }
                oarStateTexts[i].text = s;

                oarStateButtons[i].GetComponentInChildren<UnityEngine.UI.Text>().text = "Side: " + oar.side;
                i++;
            }
        }
    }

    private void FlipOar(int i)
    {
        if (oars.oars.Length > i)
        {
            if(oars.oars[i].side == WiimoteOars.Oar.SIDES.RIGHT)
            {
                oars.oars[i].side = WiimoteOars.Oar.SIDES.LEFT;
            } else
            {
                oars.oars[i].side = WiimoteOars.Oar.SIDES.RIGHT;
            }
        }
    }
}
