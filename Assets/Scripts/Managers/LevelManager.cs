using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string zoneName;
    public bool loadSaveOnlyOnRespawn;

    [HideInInspector] public static List<CheckPoint> allZoneCheckPoints;
    [HideInInspector] public static CheckPoint lastCheckPoint;
    [HideInInspector] public static List<Switch> allZoneSwitchs;

    private void Awake()
    {
        allZoneCheckPoints = new List<CheckPoint>();
        allZoneSwitchs = new List<Switch>();
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

        GameManager.LoadLevel(loadSaveOnlyOnRespawn);
    }
}
