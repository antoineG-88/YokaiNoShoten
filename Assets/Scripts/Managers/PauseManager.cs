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
    public GameObject musicSlider;
    public GameObject optionMenu;
    public GameObject buttons;

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
        Time.timeScale = 1;
        isPaused = false;
        GameData.playerManager.inControl = true;
        optionMenu.SetActive(false);
        buttons.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void ReturntoMenu()
    {
        isPaused = false;
        SceneManager.LoadScene(0);
    }
    public void SelectScrollBarWithController()
    {
        eventSystem.firstSelectedGameObject = musicSlider;
        eventSystem.SetSelectedGameObject(musicSlider);
    }
}
