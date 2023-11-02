using System.Collections;
using UnityEngine.EventSystems;
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
    [HideInInspector] public static int currentChapter;
    [HideInInspector] public static int currentStoryStep;
    [HideInInspector] public static float timeElapsedPlaying;
    [HideInInspector] public static float chapterTimeElapsedPlaying;
    [HideInInspector] public static int numberOfDeath;
    [HideInInspector] public static bool isValidForClearTime;

    [HideInInspector] public static bool gameIsPaused;
    [HideInInspector] public static bool levelIsLoading;
    [HideInInspector] public static bool isInMainMenu;
    [HideInInspector] public static bool isUsingController;
    [HideInInspector] public static bool isInFullScreen;


    private Vector2 lastMousePos;
    [HideInInspector] public static EventSystem eventSystem;
    public static bool isRespawning;
    private static bool zoneLoaded;
    public static GameManager I;
    public static RumblesManager rumblesManager;
    public static ControlsManager controlsManager;

    private void Awake()
    {
        if(I != null)
        {
            Destroy(this);
        }
        else
        {
            I = this;
            rumblesManager = GetComponent<RumblesManager>();
            RumblesManager.I = rumblesManager;
            controlsManager = GetComponent<ControlsManager>();
            ControlsManager.I = controlsManager;
            DontDestroyOnLoad(gameObject);
        }

        SetSavePath();
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            ToggleFullScreen(PlayerPrefs.GetInt("fullscreen") == 1 ? true : false);
        }
        else
        {
            ToggleFullScreen(true);
        }
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
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
            timeElapsedPlaying += Time.unscaledDeltaTime;
            chapterTimeElapsedPlaying += Time.unscaledDeltaTime;
        }

        CheckInputType();
    }

    public static void LoadLevel(bool onlyOnRespawn, int specificStart)
    {
        if(!onlyOnRespawn || (onlyOnRespawn && isRespawning))
        {
            if (!isRespawning)
            {
                GameSave loadedSave = SaveSystem.LoadGameSave();
                if (loadedSave != null)
                {
                    numberOfDeath = loadedSave.numberOfDeath;
                    timeElapsedPlaying = loadedSave.timeElapsed;
                    chapterTimeElapsedPlaying = loadedSave.chapterTimeElapsed;
                }
            }

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
        SaveSystem.SaveGameAndProgression(currentChapter);
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
                    if(LevelManager.allZoneSwitchs[i].isOn)
                    {
                        EventTrigger switchEvent = LevelManager.allZoneSwitchs[i] as EventTrigger;
                        if (switchEvent != null)
                        {
                            foreach(GameObject eventObject in switchEvent.objectsDisabledAfterEvent)
                            {
                                eventObject.SetActive(false);
                            }

                            foreach (GameObject eventObject in switchEvent.objectsEnabledAfterEvent)
                            {
                                eventObject.SetActive(true);
                            }
                        }
                    }
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
            else
            {
                chapterTimeElapsedPlaying = 0;
            }

            currentStoryStep = gameSave.currentStoryStep;
            isValidForClearTime = gameSave.isValidRun;
        }
        else
        {
            currentStoryStep = 0;
            timeElapsedPlaying = 0;
            numberOfDeath = 0;
            chapterTimeElapsedPlaying = 0;
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

    private void CheckInputType()
    {
        if (isUsingController)
        {
            if (Vector2.Distance(Input.mousePosition, lastMousePos) > 50f || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                isUsingController = false;
                Cursor.visible = true;
                eventSystem.SetSelectedGameObject(null);
            }
        }
        else
        {
            lastMousePos = Input.mousePosition;
            if (Input.GetButtonDown("AButton") || Input.GetButtonDown("BButton") || Input.GetButtonDown("XButton") || Input.GetButtonDown("YButton") || Input.GetAxisRaw("RightTrigger") == 1 || Input.GetAxisRaw("LeftTrigger") == 1
                || Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("RightStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("RightStickV")) > 0.5f)
            {
                isUsingController = true;
                Cursor.visible = false;
            }
        }
    }

    public static void ToggleFullScreen(bool isFullscreen)
    {
        isInFullScreen = isFullscreen;
        if(isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1280, 720, false);
        }
        PlayerPrefs.SetInt("fullscreen", isInFullScreen ? 1 : 0);
    }
}
