using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    private bool isPaused;
    private EventSystem eventSystem;
    public GameObject pauseButton;
    public GameObject mainVolumeSlider;
    public GameObject optionMenu;
    public GameObject buttons;

    private GameObject lastObjectSelected;

    void Start()
    {
        eventSystem = EventSystem.current;
    }
    void Update()
    {
        if (Input.GetButtonDown("StartButton") || Input.GetKeyDown(KeyCode.Escape))
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

        if(eventSystem.currentSelectedGameObject != null)
        {
            lastObjectSelected = eventSystem.currentSelectedGameObject;
        }
        else
        {
            if (Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f)
            {
                EventSystem.current.SetSelectedGameObject(lastObjectSelected);
            }
        }
    }
    public void Pause()
    {
        buttons.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        eventSystem.firstSelectedGameObject = pauseButton;
        eventSystem.SetSelectedGameObject(pauseButton);
        Time.timeScale = 0;
        isPaused = true;
        GameData.playerManager.inControl = false;
    }

    public void Resume()
    {
        optionMenu.SetActive(false);
        buttons.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        GameData.playerManager.inControl = true;
    }

    public void ReturntoMenu()
    {
        Time.timeScale = 1;
        isPaused = false;

        GameManager.I.StartCoroutine(GameManager.LoadWithProgress(0));
    }
    public void SelectScrollBarWithController()
    {
        eventSystem.firstSelectedGameObject = mainVolumeSlider;
        eventSystem.SetSelectedGameObject(mainVolumeSlider);
    }

    public void ManualRespawn()
    {
        Resume();
        GameManager.Respawn();
    }
}
