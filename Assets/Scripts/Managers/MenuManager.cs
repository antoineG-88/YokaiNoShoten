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
    // Start is called before the first frame update

    private void Start()
    {
        eventSystem = EventSystem.current;
        GameManager.LoadProgression();
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
    public void StartNewGame()
    {
        Time.timeScale = 1f;
        for (int i = 0; i < zones.Count; i++)
        {
            SaveSystem.DeleteSaveFile(zones[i].zoneName);
        }
        SceneManager.LoadScene(zones[0].zoneBuildIndex);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        GameManager.LoadProgression();
        Debug.Log("current storyStep : " + GameManager.currentStoryStep);
        for (int i = zones.Count - 1; i >= 0; i--)
        {
            if(GameManager.currentStoryStep <= zones[i].zoneMaxStoryStep && (((i - 1) > 0 && GameManager.currentStoryStep > zones[i - 1].zoneMaxStoryStep) || (i - 1) == 0))
            {
                Debug.Log("load scene : " + zones[i].zoneBuildIndex);
                SceneManager.LoadScene(zones[i].zoneBuildIndex);
                break;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salut");
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