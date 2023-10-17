using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneSave
{
    public string zoneName;
    public int lastCheckPointIndex;
    public List<bool> switchStates;
    public List<bool> enemyStates;

    public ZoneSave()
    {
        lastCheckPointIndex = LevelManager.lastCheckPoint.checkPointNumber;

        zoneName = GameManager.currentZoneName;

        switchStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
        {
            switchStates.Add(LevelManager.allZoneSwitchs[i].isOn);
        }


        enemyStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneEnemies.Count; i++)
        {
            if(!GameData.levelManager.doNotSaveEnnemiesDeath)
            {
                enemyStates.Add(LevelManager.allZoneEnemies[i] != null);
            }
            else
            {
                enemyStates.Add(true);
            }
        }
    }
}
