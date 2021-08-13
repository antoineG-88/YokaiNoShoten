using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public List<string> SceneNames;
    // Start is called before the first frame update

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        for (int i = 0; i < SceneNames.Count; i++)
        {
            SaveSystem.DeleteSaveFile(SceneNames[i]);
        }
        SceneManager.LoadScene("1");
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
}