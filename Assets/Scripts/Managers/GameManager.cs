﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Save settings")]
    public string progressionDataSaveFileName;
    public string zoneDataSaveFileNamePrefixe;
    public string zoneSaveFileExtension;
    public string progressionSaveFileExtension;
    public string defaultSaveDirectoryName;
    public string defaultGameDirectoryName;

    [HideInInspector] public static string currentZoneName;
    [HideInInspector] public static int currentStoryStep;

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

    }

    private void Start()
    {
        SetSavePath();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene(2);
        }


        if (Input.GetKeyDown(KeyCode.Delete))
        {
            SaveSystem.DeleteSaveFile(currentZoneName);
        }
    }

    public static void LoadLevel(bool onlyOnRespawn, int specificStart)
    {
        if(!onlyOnRespawn || (onlyOnRespawn && isRespawning))
        {
            isRespawning = false;
            LoadProgression();
        }
        else if(specificStart >= 0)
        {
            SetSpecificStart(specificStart);
        }
    }

    public static void Respawn()
    {
        isRespawning = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void SaveProgression(CheckPoint checkPoint)
    {
        LevelManager.lastCheckPoint = checkPoint;
        SaveSystem.SaveProgression(currentZoneName);
    }

    public static void LoadProgression()
    {
        ZoneData zoneData = SaveSystem.LoadZone(currentZoneName);
        if(zoneData != null)
        {
            for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
            {
                LevelManager.allZoneSwitchs[i].isOn = zoneData.switchStates[i];
            }

            for (int i = 0; i < LevelManager.allZoneCheckPoints.Count; i++)
            {
                if (LevelManager.allZoneCheckPoints[i].checkPointNumber == zoneData.lastCheckPointIndex)
                {
                    GameData.player.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
                    Camera.main.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
                }
            }
        }

        ProgressionData progressionData = SaveSystem.LoadProgression();
        if(progressionData != null)
        {
            currentStoryStep = progressionData.currentStoryStep;
        }
    }

    public static void SetSpecificStart(int checkpointIndex)
    {
        for (int i = 0; i < LevelManager.allZoneCheckPoints.Count; i++)
        {
            if (LevelManager.allZoneCheckPoints[i].checkPointNumber == checkpointIndex)
            {
                GameData.player.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
                Camera.main.transform.position = LevelManager.allZoneCheckPoints[i].transform.position;
            }
        }
    }

    private void SetSavePath()
    {
        SaveSystem.progressionDataSaveFileName = progressionDataSaveFileName;
        SaveSystem.zoneSaveFileExtension = zoneSaveFileExtension;
        SaveSystem.progressionSaveFileExtension = progressionSaveFileExtension;
        SaveSystem.defaultGameDirectoryName = defaultGameDirectoryName;
        SaveSystem.defaultSaveDirectoryName = defaultSaveDirectoryName;
        SaveSystem.zoneDataFileNamePrefixe = zoneDataSaveFileNamePrefixe;
        SaveSystem.SetSavePath(string.Empty);
    }

    public static void LoadNewZone(int sceneIndex)
    {
        SaveProgression(LevelManager.lastCheckPoint);
        SceneManager.LoadScene(sceneIndex);
    }
}
