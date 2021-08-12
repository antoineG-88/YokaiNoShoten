using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Zone Transition")]
    public Color transitionScreenColor;
    public float transitionTime;
    [Header("Deco")]
    public int decoSceneIndex;
    public string decoScenePath;
    [Header("Save Management")]
    public string zoneName;
    public bool loadSaveOnlyOnRespawn;
    public int specificCheckpointStart;

    public static List<CheckPoint> allZoneCheckPoints;
    public static CheckPoint lastCheckPoint;
    public static List<Switch> allZoneSwitchs;
    [HideInInspector] public Scene decoScene;
    private int zoneLoadCountDown;

    private void Awake()
    {
        LoadDecoScene();
        allZoneCheckPoints = new List<CheckPoint>();
        allZoneSwitchs = new List<Switch>();
        zoneLoadCountDown = 2;
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
        StartCoroutine(StartBlackScreenFade());
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

    public IEnumerator LoadNewZone(int zoneBuildIndex)
    {
        float timer = 0;
        while (timer < transitionTime)
        {
            BlackScreenManager.blackScreen.color = Color.Lerp(Color.clear, transitionScreenColor, timer / transitionTime);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        BlackScreenManager.blackScreen.color = transitionScreenColor;

        GameManager.LoadNewZone(zoneBuildIndex);
    }

    private IEnumerator StartBlackScreenFade()
    {
        float timer = 0;
        BlackScreenManager.blackScreen.color = transitionScreenColor;
        while (timer < transitionTime)
        {
            BlackScreenManager.blackScreen.color = Color.Lerp(transitionScreenColor, Color.clear, timer / transitionTime);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        BlackScreenManager.blackScreen.color = Color.clear;
    }
}

