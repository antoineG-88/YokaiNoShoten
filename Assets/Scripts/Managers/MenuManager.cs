using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public List<Zone> zones;
    private EventSystem eventSystem;
    public GameObject continueButton;
    public GameObject startNewGameButton;
    public GameObject scrollBar;
    public GameObject mainMenu;
    public GameObject optionMenu;
    public GameObject creditsMenu;
    public Text continueInfo;
    public GameObject warnWindow;
    public GameObject defaultSelectedWarnButton;
    public GameObject otherWarnButton;
    [Space]
    public GameObject chaptersMenu;
    public Button[] chaptersButtons;
    public Text[] chaptersName;
    public Text[] chaptersClearTime;
    public Text bestClearTime;
    [Header("Transition Options")]
    public float transitionFadeTime;
    public Animator seikiMenu;
    public float timeBeforeFade;

    private GameObject lastSelectedObject;
    private bool isGameStarting;

    private void Start()
    {
        eventSystem = EventSystem.current;
        GameManager.isInMainMenu = true;
        GameSave loadedSave = SaveSystem.LoadGameSave();
        if (loadedSave != null)
        {
            GameManager.currentStoryStep = loadedSave.currentStoryStep;
            GameManager.numberOfDeath = loadedSave.numberOfDeath;
            GameManager.timeElapsedPlaying = loadedSave.timeElapsed;

        }
        GameManager.eventSystem = EventSystem.current;
        UpdateContinueButton();
    }

    private void Update()
    {

        if (eventSystem.currentSelectedGameObject == null)
        {
            if (GameManager.isUsingController)
            {
                if (Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f)
                {
                    SelectButtonWithController(lastSelectedObject);
                }
            }
        }
        else
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
        }

        if((Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape)) && !isGameStarting)
        {
            mainMenu.SetActive(true);
            optionMenu.SetActive(false);
            creditsMenu.SetActive(false);
            chaptersMenu.SetActive(false);
            warnWindow.SetActive(false);

            UpdateContinueButton();
        }
    }

    private void UpdateContinueButton()
    {
        GameSave loadedSave = SaveSystem.LoadGameSave();
        if (loadedSave == null || SaveSystem.LoadGameSave().currentStoryStep >= zones[zones.Count - 1].zoneMaxStoryStep)
        {
            continueButton.SetActive(false);
            continueInfo.text = "";
            SelectButtonWithController(startNewGameButton);
        }
        else
        {
            SelectButtonWithController(continueButton);

            continueInfo.text = zones[loadedSave._chapterToLoad - 1].zoneName + "\n"
                + (loadedSave.isValidRun ? (
                (GameManager.GetHourFromSecondElapsed(loadedSave.timeElapsed) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(loadedSave.timeElapsed) + "hours - "))
                + GameManager.GetMinutesFromSecondElapsed(loadedSave.timeElapsed) + "min - "
                + GameManager.GetSecondsFromSecondElapsed(loadedSave.timeElapsed) + "seconds") : "");
        }
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        if(SaveSystem.LoadGameSave() != null)
        {
            WarnPlayerForDeleteSave(0);
        }
        else
        {
            StartCoroutine(StartTransition(0));
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        GameSave loadedSave = SaveSystem.LoadGameSave();
        if (loadedSave != null)
            GameManager.currentStoryStep = loadedSave.currentStoryStep;


        StartCoroutine(StartTransition(loadedSave._chapterToLoad - 1));

        /*
        for (int i = 0; i < zones.Count; i++)
        {
            if(zones[i].zoneName == loadedSave.lastZoneData.zoneName)
            {
                StartCoroutine(StartTransition(i));
                break;
            }
        }*/
    }

    private IEnumerator StartTransition(int zoneIndexToLoad)
    {
        isGameStarting = true;
        mainMenu.SetActive(false);
        optionMenu.SetActive(false);
        creditsMenu.SetActive(false);
        chaptersMenu.SetActive(false);
        warnWindow.SetActive(false);

        seikiMenu.SetTrigger("Jump");
        yield return new WaitForSeconds(timeBeforeFade);
        float timer = 0;
        while (timer < transitionFadeTime)
        {
            BlackScreenManager.SetAlpha(timer / transitionFadeTime);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        BlackScreenManager.SetAlpha(1);

        StartCoroutine(GameManager.LoadWithProgress(zones[zoneIndexToLoad].zoneBuildIndex));
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    public void SelectButtonWithController(GameObject objectToSelect)
    {
        eventSystem.firstSelectedGameObject = objectToSelect;
        eventSystem.SetSelectedGameObject(objectToSelect);
    }
    public void SelectScrollBarWithController()
    {
        eventSystem.firstSelectedGameObject = scrollBar;
        eventSystem.SetSelectedGameObject(scrollBar);
    }

    public void UpdateChaptersAvailable()
    {
        ProgressionSave progressionSave = SaveSystem.LoadProgressionSave();
        if (progressionSave != null && progressionSave.hasFinishedTheGame)
        {
            float playTime = progressionSave.fastestClearTime;
            bestClearTime.text = "Fastest clear time : \n" + (GameManager.GetHourFromSecondElapsed(playTime) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(playTime) + "hours - "))
            + GameManager.GetMinutesFromSecondElapsed(playTime) + "min - "
            + (GameManager.GetSecondsFromSecondElapsed(playTime) + GameManager.GetSubSecondFromSecondElapsed(playTime)).ToString("0.00") + " seconds";
        }
        else
        {
            bestClearTime.text = "Fastest clear time : \n Game not yet completed";
            chaptersButtons[0].interactable = true;
            for (int i = 1; i < chaptersButtons.Length; i++)
            {
                if (progressionSave != null)
                {
                    chaptersButtons[i].interactable = progressionSave.chapterReached > i;
                    chaptersName[i].CrossFadeAlpha(progressionSave.chapterReached > i ? 1 : 0, 0, false);
                }
                else
                {
                    chaptersButtons[i].interactable = false;
                    chaptersName[i].CrossFadeAlpha(0, 0, false);
                }
            }
        }

        for (int i = 0; i < chaptersClearTime.Length; i++)
        {
            if (progressionSave != null)
            {
                if (progressionSave.chaptersClearTime[i] != 0)
                {
                    chaptersClearTime[i].text = (GameManager.GetHourFromSecondElapsed(progressionSave.chaptersClearTime[i]) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(progressionSave.chaptersClearTime[i]) + "hours - "))
                    + GameManager.GetMinutesFromSecondElapsed(progressionSave.chaptersClearTime[i]) + "min - "
                    + (GameManager.GetSecondsFromSecondElapsed(progressionSave.chaptersClearTime[i]) + GameManager.GetSubSecondFromSecondElapsed(progressionSave.chaptersClearTime[i])).ToString("0.00") + " seconds";
                }
                else
                {
                    chaptersClearTime[i].text = "";
                }
            }
            else
            {
                chaptersClearTime[i].text = "";
            }
        }
    }

    public void StartGameAtSpecificChapter(string chapterName)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].zoneName == chapterName)
            {
                GameManager.currentStoryStep = 0;
                if(SaveSystem.LoadGameSave() != null)
                {
                    WarnPlayerForDeleteSave(i);
                }
                else
                {
                    StartCoroutine(StartTransition(i));
                }

                break;
            }
        }
    }

    private int sceneToLoadWaitingForWarn;
    //private bool isBeingWarned;
    private GameObject selectedButtonWhenWarn;
    public void WarnPlayerForDeleteSave(int sceneIndex)
    {
        warnWindow.SetActive(true);
        selectedButtonWhenWarn = eventSystem.currentSelectedGameObject;
        sceneToLoadWaitingForWarn = sceneIndex;
        SelectButtonWithController(defaultSelectedWarnButton);
        //isBeingWarned = true;
    }

    public void ValidateDeletion()
    {
        //isBeingWarned = false;
        SaveSystem.DeleteGameSaveFile();
        StartCoroutine(StartTransition(sceneToLoadWaitingForWarn));
        warnWindow.SetActive(false);
    }

    public void CancelDeletion()
    {
        //isBeingWarned = false;
        sceneToLoadWaitingForWarn = 0;
        warnWindow.SetActive(false);
        eventSystem.SetSelectedGameObject(selectedButtonWhenWarn);
    }

    [System.Serializable]
    public class Zone
    {
        public string zoneName;
        public int zoneBuildIndex;
        public int zoneMaxStoryStep;
    }
}