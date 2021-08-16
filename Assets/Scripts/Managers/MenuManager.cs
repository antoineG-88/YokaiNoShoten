using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public List<string> SceneNames;
    private EventSystem eventSystem;
    public GameObject playButton;
    public GameObject scrollBar;
    // Start is called before the first frame update

    private void Start()
    {
        eventSystem = EventSystem.current;
        SelectButtonWithController();
    }
    public void StartNewGame()
    {
        Time.timeScale = 1f;
        for (int i = 0; i < SceneNames.Count; i++)
        {
            SaveSystem.DeleteSaveFile(SceneNames[i]);
        }
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        SaveSystem.LoadProgression();
        //insérer le code pour continuer le jeu
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salut");
    }
    public void SelectButtonWithController()
    {
        eventSystem.firstSelectedGameObject = playButton;
        eventSystem.SetSelectedGameObject(playButton);
    }
    public void SelectScrollBarWithController()
    {
        eventSystem.firstSelectedGameObject = scrollBar;
        eventSystem.SetSelectedGameObject(scrollBar);
    }
}