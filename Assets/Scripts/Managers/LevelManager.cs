using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int decoSceneIndex;
    public string decoScenePath;
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
}

