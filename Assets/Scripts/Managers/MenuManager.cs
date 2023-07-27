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


    private void Start()
    {
        eventSystem = EventSystem.current;

        if(SaveSystem.LoadProgression() != null)
        {
            GameManager.currentStoryStep = SaveSystem.LoadProgression().currentStoryStep;
        }

        if(GameManager.currentStoryStep == 0)
        {
            continueButton.SetActive(false);
            SelectButtonWithController(startNewGameButton);
        }
        else
        {
            SelectButtonWithController(continueButton);
        }
    }

    private void Update()
    {
        if(eventSystem.currentSelectedGameObject == null)
        {
            if(Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f)
            {
                if (GameManager.currentStoryStep == 0)
                {
                    continueButton.SetActive(false);
                    SelectButtonWithController(startNewGameButton);
                }
                else
                {
                    SelectButtonWithController(continueButton);
                }
            }
        }

        if(Input.GetButtonDown("BButton"))
        {
            mainMenu.SetActive(true);
            optionMenu.SetActive(false);
            creditsMenu.SetActive(false);
            
            if (GameManager.currentStoryStep == 0)
            {
                continueButton.SetActive(false);
                SelectButtonWithController(startNewGameButton);
            }
            else
            {
                SelectButtonWithController(continueButton);
            }
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
        if (SaveSystem.LoadProgression() != null)
            GameManager.currentStoryStep = SaveSystem.LoadProgression().currentStoryStep;
        for (int i = 0; i < zones.Count; i++)
        {
            if(GameManager.currentStoryStep <= zones[i].zoneMaxStoryStep)
            {
                StartCoroutine(StartTransition(i));
                break;
            }
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