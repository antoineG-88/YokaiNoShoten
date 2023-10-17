using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    [Header("Transition Options")]
    public float transitionFadeTime;
    public Animator seikiMenu;
    public float timeBeforeFade;

    private GameObject lastSelectedObject;

    private void Start()
    {
        eventSystem = EventSystem.current;
        GameManager.isInMainMenu = true;

        if(SaveSystem.LoadGameSave() != null)
        {
            GameManager.currentStoryStep = SaveSystem.LoadGameSave().currentStoryStep;
            GameManager.numberOfDeath = SaveSystem.LoadGameSave().numberOfDeath;
            GameManager.timeElapsedPlaying = SaveSystem.LoadGameSave().timeElapsed;
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

        if(Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape))
        {
            mainMenu.SetActive(true);
            optionMenu.SetActive(false);
            creditsMenu.SetActive(false);

            UpdateContinueButton();
        }
    }

    private void UpdateContinueButton()
    {
        if (SaveSystem.LoadGameSave() == null)
        {
            continueButton.SetActive(false);
            SelectButtonWithController(startNewGameButton);
        }
        else
        {
            SelectButtonWithController(continueButton);
        }
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        for (int i = 0; i < zones.Count; i++)
        {
            SaveSystem.DeleteSaveFile(zones[i].zoneName);
        }
        StartCoroutine(StartTransition(0));
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

    [System.Serializable]
    public class Zone
    {
        public string zoneName;
        public int zoneBuildIndex;
        public int zoneMaxStoryStep;
    }
}