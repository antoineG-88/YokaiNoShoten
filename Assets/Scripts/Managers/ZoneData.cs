using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneData
{
    public string currentZoneName;
    public int lastCheckPointIndex;
    public List<bool> switchStates;
    public List<bool> enemyStates;

    public ZoneData()
    {
        lastCheckPointIndex = LevelManager.lastCheckPoint.checkPointNumber;

        currentZoneName = GameManager.currentZoneName;

        switchStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
        {
            switchStates.Add(LevelManager.allZoneSwitchs[i].isOn);
        }


        enemyStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneEnemies.Count; i++)
        {
            enemyStates.Add(LevelManager.allZoneEnemies[i] != null);
        }
    }
}
