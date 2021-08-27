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

        if(SaveSystem.LoadProgression() != null)
            GameManager.currentStoryStep = SaveSystem.LoadProgression().currentStoryStep;

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
        if (SaveSystem.LoadProgression() != null)
            GameManager.currentStoryStep = SaveSystem.LoadProgression().currentStoryStep;
        for (int i = 0; i < zones.Count; i++)
        {
            if(GameManager.currentStoryStep <= zones[i].zoneMaxStoryStep)
            {
                SceneManager.LoadScene(zones[i].zoneBuildIndex);
                break;
            }
        }
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