using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour {
    public GameObject Panel;

    UI ui;

    private void Awake()
    {
        ui = FindObjectOfType<UI>();
        Panel.SetActive(false);
    }

    void Update () {
	    if(Input.GetKeyDown(KeyCode.F1))
        {
            Panel.SetActive(!Panel.activeInHierarchy);
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
}
