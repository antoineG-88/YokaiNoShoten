using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Save settings")]
    public string progressionSaveFileName;
    public string gameSaveFileName;
    public string saveFileExtension;
    public string defaultSaveDirectoryName;
    public string defaultGameDirectoryName;
    [Header("________")]
    public bool enableZonesKeyShortcuts;

    [HideInInspector] public static string currentZoneName;
    [HideInInspector] public static int currentStoryStep;
    [HideInInspector] public static float timeElapsedPlaying;
    [HideInInspector] public static int numberOfDeath;

    [HideInInspector] public static bool gameIsPaused;
    [HideInInspector] public static bool levelIsLoading;
    [HideInInspector] public static bool isInMainMenu;

    public static bool isRespawning;
    private static bool zoneLoaded;
    public static GameManager I;

    private void Awake()
    {
        if(I != null)
        {
            Destroy(this);
        }
        else
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }

        SetSavePath();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(enableZonesKeyShortcuts)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(LoadWithProgress(1));
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(LoadWithProgress(3));
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(LoadWithProgress(5));
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                StartCoroutine(LoadWithProgress(6));
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                StartCoroutine(LoadWithProgress(7));
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                StartCoroutine(LoadWithProgress(9));
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                StartCoroutine(LoadWithProgress(11));
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            SaveSystem.DeleteGameSaveFile();
        }

        if(!gameIsPaused && !levelIsLoading && !isInMainMenu)
        {
            timeElapsedPlaying += Time.deltaTime;
        }
    }

    public static void LoadLevel(bool onlyOnRespawn, int specificStart)
    {
        if(!onlyOnRespawn || (onlyOnRespawn && isRespawning))
        {
            isRespawning = false;
            LoadZoneSave();
        }
        else if(specificStart >= 0)
        {
            SetSpecificStart(specificStart);
        }
    }

    public static void Respawn(bool increaseDeathCount)
    {
        isRespawning = true;
        if(increaseDeathCount)
            numberOfDeath++;
        GameData.slowMoManager.StopSlowMo();
        I.StartCoroutine(LoadWithProgress(SceneManager.GetActiveScene().buildIndex));
    }

    public static void SaveProgression(CheckPoint checkPoint)
    {
        LevelManager.lastCheckPoint = checkPoint;
        SaveSystem.SaveGameAndProgression(currentZoneName);
    }

    public static void LoadZoneSave()
    {
        GameSave gameSave = SaveSystem.LoadGameSave();

        if(gameSave != null)
        {
            ZoneSave zoneSave = null;
            if (gameSave.lastZoneData.zoneName == currentZoneName)
            {
                zoneSave = gameSave.lastZoneData;
            }

            if (zoneSave != null)
            {
                for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
                {
                    LevelManager.allZoneSwitchs[i].isOn = zoneSave.switchStates[i];
                    LevelManager.allZoneSwitchs[i].saveState = zoneSave.switchStates[i];
                }

                for (int i = 0; i < LevelManager.allZoneEnemies.Count; i++)
                {
                    if (!zoneSave.enemyStates[i])
                    {
                        Destroy(LevelManager.allZoneEnemies[i].gameObject);
                    }
                }

                for (int i = 0; i < LevelManager.allZoneCheckPoints.Count; i++)
                {
                    if (LevelManager.allZoneCheckPoints[i].checkPointNumber == zoneSave.lastCheckPointIndex)
                    {
                        GameData.player.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
                        GameData.mainCamera.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
                    }
                }
            }

            currentStoryStep = gameSave.currentStoryStep;
        }
        else
        {
            currentStoryStep = 0;
            timeElapsedPlaying = 0;
            numberOfDeath = 0;
        }
    }

    public static void SetSpecificStart(int checkpointIndex)
    {
        for (int i = 0; i < LevelManager.allZoneCheckPoints.Count; i++)
        {
            if (LevelManager.allZoneCheckPoints[i].checkPointNumber == checkpointIndex)
            {
                GameData.player.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
                GameData.mainCamera.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
            }
        }
    }

    private void SetSavePath()
    {
        SaveSystem.progressionSaveFileName = progressionSaveFileName;
        SaveSystem.saveFileExtension = saveFileExtension;
        SaveSystem.defaultGameDirectoryName = defaultGameDirectoryName;
        SaveSystem.defaultSaveDirectoryName = defaultSaveDirectoryName;
        SaveSystem.gameSaveFileName = gameSaveFileName;
        SaveSystem.SetSavePath(string.Empty);
    }

    public static void LoadNewZone(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public static IEnumerator LoadWithProgress(int sceneIndex)
    {
        levelIsLoading = true;
        BlackScreenManager.SetLoadActive(true);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while(!loadOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        BlackScreenManager.SetLoadActive(false);
    }



    public static int GetHourFromSecondElapsed(float secondsToConvert)
    {
        return Mathf.FloorToInt(secondsToConvert) / 3600;
    }

    public static int GetMinutesFromSecondElapsed(float secondsToConvert)
    {
        return (Mathf.FloorToInt(secondsToConvert) % 3600) / 60;
    }

    public static int GetSecondsFromSecondElapsed(float secondsToConvert)
    {
        return Mathf.FloorToInt(secondsToConvert) % 3600 % 60;
    }

    public static float GetSubSecondFromSecondElapsed(float secondsToConvert)
    {
        return secondsToConvert - Mathf.FloorToInt(secondsToConvert);
    }
}
