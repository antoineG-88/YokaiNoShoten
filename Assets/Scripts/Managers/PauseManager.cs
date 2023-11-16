using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [HideInInspector] public bool isPaused;
    private EventSystem eventSystem;
    public GameObject pauseBackground;
    public GameObject pauseButton;
    public GameObject mainVolumeSlider;
    public GameObject optionMenu;
    public GameObject optionMainWindow;
    public GameObject buttons;
    public Text playTimeText;
    public Text chapterTimeText;
    public Text deathCountText;
    public Text storyStep;
    public Text currentChapterText;
    public Text saveInfoText;
    public UISoundManager uISoundManager;
    public AudioClip pauseSound;
    public AudioClip resumeSound;

    private GameObject lastObjectSelected;

    void Start()
    {
        eventSystem = EventSystem.current;

        GameManager.gameIsPaused = false;
    }
    void Update()
    {
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

        if(isPaused)
        {
            if (Input.GetButtonDown("StartButton"))
            {
                Resume();
            }

            if (Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape))
            {
                if(buttons.activeSelf)
                    Resume();


                if (optionMenu.activeSelf && optionMainWindow.activeSelf)
                    Pause();
            }

            if (GameManager.isValidForClearTime)
            {
                float playTime = GameManager.timeElapsedPlaying;
                playTimeText.text = (GameManager.GetHourFromSecondElapsed(playTime) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(playTime) + "hours - "))
                + GameManager.GetMinutesFromSecondElapsed(playTime) + "min - "
                + (GameManager.GetSecondsFromSecondElapsed(playTime) + GameManager.GetSubSecondFromSecondElapsed(playTime)).ToString("0.00") + " seconds";
            }
            else
            {

                playTimeText.text = "Not valid run";
            }

            float chapterPlayTime = GameManager.chapterTimeElapsedPlaying;
            chapterTimeText.text = "Chapter time elapsed : " + (GameManager.GetHourFromSecondElapsed(chapterPlayTime) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(chapterPlayTime) + "hours - "))
            + GameManager.GetMinutesFromSecondElapsed(chapterPlayTime) + "min - "
            + (GameManager.GetSecondsFromSecondElapsed(chapterPlayTime) + GameManager.GetSubSecondFromSecondElapsed(chapterPlayTime)).ToString("0.00") + " seconds";

            deathCountText.text = GameManager.numberOfDeath.ToString();
            storyStep.text = GameManager.currentStoryStep.ToString();
            currentChapterText.text = GameManager.currentZoneName;


        }
        else
        {
            if (Input.GetButtonDown("StartButton") || Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus && !isPaused)
        {
            Pause();
        }
    }

    public void Pause()
    {
        uISoundManager.PlayUISound(pauseSound);
        pauseBackground.SetActive(true);
        buttons.SetActive(true);
        optionMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        eventSystem.firstSelectedGameObject = pauseButton;
        eventSystem.SetSelectedGameObject(pauseButton);
        Time.timeScale = 0;
        isPaused = true;
        GameData.playerManager.inControl = false;
        GameManager.gameIsPaused = true;
        if(!GameManager.isUsingController)
            Cursor.visible = true;

        if (SaveSystem.lastSaveTime != 0)
        {
            saveInfoText.text = "Game saved " + Mathf.Round(Time.realtimeSinceStartup - SaveSystem.lastSaveTime).ToString() + " seconds ago";
        }
        else
        {
            saveInfoText.text = "Game not saved yet";
        }
    }

    public void Resume()
    {
        uISoundManager.PlayUISound(resumeSound);
        optionMenu.SetActive(false);
        buttons.SetActive(false);
        pauseBackground.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        GameData.playerManager.inControl = true;
        GameManager.gameIsPaused = false;
        Cursor.visible = false;
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

    public void SelectObjectWithController(GameObject objectToSelect)
    {
        eventSystem.firstSelectedGameObject = objectToSelect;
        eventSystem.SetSelectedGameObject(objectToSelect);
    }

    public void ManualRespawn()
    {
        Resume();
        GameManager.Respawn(false);
    }
}
