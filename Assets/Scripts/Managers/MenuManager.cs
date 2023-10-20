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
    public GameObject warnWindowContinue;
    [Space]
    public GameObject chaptersMenu;
    public Button[] chaptersButtons;
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

        UpdateContinueButton();
    }

    private void Update()
    {
        
        if(eventSystem.currentSelectedGameObject == null)
        {
            if(Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f)
            {
                SelectButtonWithController(lastSelectedObject);
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

            continueInfo.text = loadedSave.lastZoneData.zoneName + "\n"
                + (GameManager.GetHourFromSecondElapsed(loadedSave.timeElapsed) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(loadedSave.timeElapsed) + "hours - "))
                + GameManager.GetMinutesFromSecondElapsed(loadedSave.timeElapsed) + "min - "
                + GameManager.GetSecondsFromSecondElapsed(loadedSave.timeElapsed) + "seconds";
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
        for (int i = 0; i < zones.Count; i++)
        {
            if(zones[i].zoneName == loadedSave.lastZoneData.zoneName)
            {
                StartCoroutine(StartTransition(i));
                break;
            }
            /*
            if(GameManager.currentStoryStep <= zones[i].zoneMaxStoryStep)
            {
                StartCoroutine(StartTransition(i));
                break;
            }*/
        }
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
            bestClearTime.text = "Fastest clear time : " + (GameManager.GetHourFromSecondElapsed(playTime) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(playTime) + "hours - "))
            + GameManager.GetMinutesFromSecondElapsed(playTime) + "min - "
            + (GameManager.GetSecondsFromSecondElapsed(playTime) + GameManager.GetSubSecondFromSecondElapsed(playTime)).ToString("0.00") + " seconds";
        }
        else
        {
            bestClearTime.text = "";
            chaptersButtons[0].interactable = true;
            for (int i = 1; i < chaptersButtons.Length; i++)
            {
                if (progressionSave != null)
                {
                    chaptersButtons[i].interactable = progressionSave.chapterReached > i;
                }
                else
                {
                    chaptersButtons[i].interactable = false;
                }
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
    private GameObject selectedButtonWhenWarn;
    public void WarnPlayerForDeleteSave(int sceneIndex)
    {
        warnWindow.SetActive(true);
        selectedButtonWhenWarn = eventSystem.currentSelectedGameObject;
        SelectButtonWithController(warnWindowContinue);
        sceneToLoadWaitingForWarn = sceneIndex;
    }

    public void ValidateDeletion()
    {
        SaveSystem.DeleteGameSaveFile();
        StartCoroutine(StartTransition(sceneToLoadWaitingForWarn));
        warnWindow.SetActive(false);
    }

    public void CancelDeletion()
    {
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