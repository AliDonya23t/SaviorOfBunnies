using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadNextScene();
    }
}
