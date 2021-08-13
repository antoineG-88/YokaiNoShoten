using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused;
    private EventSystem eventSystem;
    public GameObject pauseButton;

    void Start()
    {
        eventSystem = EventSystem.current;
    }
    void Update()
    {
        if (Input.GetButtonDown("StartButton"))
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        GameData.playerManager.inControl = false;
        eventSystem.firstSelectedGameObject = pauseButton;
        eventSystem.SetSelectedGameObject(pauseButton);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        GameData.playerManager.inControl = true;
    }

    public void ReturntoMenu()
    {
        isPaused = false;
        SceneManager.LoadScene(0);
    }
}
