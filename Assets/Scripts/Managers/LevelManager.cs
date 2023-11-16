﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    [Header("Zone Transition")]
    public Color transitionScreenColor;
    public float transitionTime;
    [HideInInspector] public Text chapterNameDisplay;
    [Header("Deco")]
    public int decoSceneIndex;
    public string decoScenePath;
    [Header("Save Management")]
    public string zoneName;
    public int chapterNumber;
    public bool loadSaveOnlyOnRespawn;
    public int specificCheckpointStart;
    public bool doNotSaveEnnemiesDeath;

    public static List<CheckPoint> allZoneCheckPoints;
    public static CheckPoint lastCheckPoint;
    public static List<Switch> allZoneSwitchs;
    public static List<Enemy> allZoneEnemies;
    [HideInInspector] public Scene decoScene;
    private int zoneLoadCountDown;

    public static Image saveLogo;
    public static LevelManager I;

    private void Awake()
    {
        LoadDecoScene();
        allZoneCheckPoints = new List<CheckPoint>();
        allZoneSwitchs = new List<Switch>();
        allZoneEnemies = new List<Enemy>();
        GetAllZoneSwitchesAndEnemies();
        zoneLoadCountDown = 2;
        I = this;

        saveLogo = GameObject.Find("SaveLogo").GetComponent<Image>();
        saveLogo.CrossFadeAlpha(0f, 0f, false);

        GameManager.eventSystem = EventSystem.current;
    }

    void Start()
    {
        if (zoneName == "")
        {
            GameManager.currentZoneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            GameManager.currentZoneName = zoneName;
        }
        GameManager.currentChapter = chapterNumber;

        if(chapterNumber == 1)
        {
            GameManager.isValidForClearTime = true;
        }

        StartCoroutine(StartBlackScreenFade());
        GameData.audioManager.DisableNoGravityMixerEffects();

        StartCoroutine(DisplayZoneName());

        GameManager.isInMainMenu = false;
    }

    private void Update()
    {
        if(zoneLoadCountDown > 0)
        {
            zoneLoadCountDown--;
        }
        else if(zoneLoadCountDown == 0)
        {
            zoneLoadCountDown--;

            GameManager.levelIsLoading = false;
            GameManager.LoadLevel(loadSaveOnlyOnRespawn, specificCheckpointStart);
        }
    }

    public void LoadDecoScene()
    {
        if (decoSceneIndex != 0)
        {
            SceneManager.LoadScene(decoSceneIndex, LoadSceneMode.Additive);
        }
    }

    public static void ActivateSingleCheckPoint(CheckPoint singleCheckpoint)
    {
        for (int i = 0; i < allZoneCheckPoints.Count; i++)
        {
            if(allZoneCheckPoints[i] != singleCheckpoint)
            {
                allZoneCheckPoints[i].isActivated = false;
            }
            else
            {
                allZoneCheckPoints[i].isActivated = true;
            }
        }
    }

    public IEnumerator LoadNewZone(int zoneBuildIndex, int nextChapterNumber)
    {
        SaveSystem.SaveGameAndProgression(nextChapterNumber);
        SaveSystem.SaveChapterClearTime();
        float timer = 0;
        while (timer < transitionTime)
        {
            BlackScreenManager.SetAlpha(timer / transitionTime);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        BlackScreenManager.SetAlpha(1);

        //GameManager.LoadNewZone(zoneBuildIndex);
        StartCoroutine(GameManager.LoadWithProgress(zoneBuildIndex));
    }

    private IEnumerator StartBlackScreenFade()
    {
        float timer = 0;
        BlackScreenManager.SetAlpha(1);
        BlackScreenManager.blackScreen.color = transitionScreenColor;
        yield return new WaitForSeconds(0.2f);
        while (timer < transitionTime)
        {
            BlackScreenManager.SetAlpha(1 - (timer / transitionTime));

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        BlackScreenManager.SetAlpha(0);
    }

    private void GetAllZoneSwitchesAndEnemies()
    {
        GameObject[] sceneRootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < sceneRootGameObjects.Length; i++)
        {
            SearchSwitchesAndEnemiesIn(sceneRootGameObjects[i].transform);
        }
    }

    private void SearchSwitchesAndEnemiesIn(Transform parent)
    {
        Switch potentialSwitch = parent.GetComponent<Switch>();
        if(potentialSwitch != null)
        {
            allZoneSwitchs.Add(potentialSwitch);
        }

        Enemy potentialEnemy = parent.GetComponent<Enemy>();
        if (potentialEnemy != null)
        {
            allZoneEnemies.Add(potentialEnemy);
            potentialEnemy.zoneIndex = allZoneEnemies.Count - 1;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            SearchSwitchesAndEnemiesIn(parent.GetChild(i));
        }
    }

    public IEnumerator DisplayZoneName()
    {
        chapterNameDisplay = GameObject.Find("ZoneNameDisplay").GetComponent<Text>();

        chapterNameDisplay.CrossFadeAlpha(0f, 0f, false);
        chapterNameDisplay.text = zoneName;
        yield return new WaitForSeconds(0.7f);

        chapterNameDisplay.CrossFadeAlpha(1f, 0.5f, false);

        yield return new WaitForSeconds(4.0f);

        chapterNameDisplay.CrossFadeAlpha(0f, 0.5f, false);
    }

    public static void StartSaveDisplayAnim()
    {
        I.StartCoroutine(I.PlaySaveGameLogo());
    }

    private IEnumerator PlaySaveGameLogo()
    {

        saveLogo.CrossFadeAlpha(1f, 0.3f, false);

        yield return new WaitForSeconds(1.5f);

        saveLogo.CrossFadeAlpha(0f, 0.3f, false);
    }
}

