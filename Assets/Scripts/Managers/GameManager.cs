using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string zoneName;
    [Header("Save settings")]
    public string progressionDataSaveFileNamePrefixe;
    public string saveFileExtension;
    public string defaultSaveDirectoryName;
    public string defaultGameDirectoryName;

    [HideInInspector] public static string currentZoneName;
    [HideInInspector] public static List<CheckPoint> allZoneCheckPoints;
    [HideInInspector] public static CheckPoint lastCheckPoint;
    [HideInInspector] public static List<Switch> allZoneSwitchs;
    [HideInInspector] public static int currentStoryStep;

    private bool zoneLoaded;
    private void Awake()
    {
        allZoneCheckPoints = new List<CheckPoint>();
        allZoneSwitchs = new List<Switch>();
    }

    private void Start()
    {
        SetSavePath();
        if(zoneName == "")
        {
            currentZoneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            currentZoneName = zoneName;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(1);
        }
        if(!zoneLoaded && allZoneSwitchs.Count > 0 && allZoneCheckPoints.Count > 0)
        {
            zoneLoaded = true;
            LoadProgression();
        }
    }

    public static void SaveProgression(CheckPoint checkPoint)
    {
        lastCheckPoint = checkPoint;
        SaveSystem.SaveProgression();
    }

    public static void LoadProgression()
    {
        ProgressionData progressionData = SaveSystem.LoadProgression();
        for (int i = 0; i < allZoneSwitchs.Count; i++)
        {
            allZoneSwitchs[i].isOn = progressionData.switchStates[i];
        }
        GameData.player.transform.position = allZoneCheckPoints[progressionData.lastCheckPointIndex].transform.position;
        currentStoryStep = progressionData.currentStoryStep;
    }

    private void SetSavePath()
    {
        SaveSystem.progressionDataSaveFileNamePrefixe = progressionDataSaveFileNamePrefixe;
        SaveSystem.saveFileExtension = saveFileExtension;
        SaveSystem.defaultGameDirectoryName = defaultGameDirectoryName;
        SaveSystem.defaultSaveDirectoryName = defaultSaveDirectoryName;
        SaveSystem.SetSavePath(string.Empty);
    }
}
