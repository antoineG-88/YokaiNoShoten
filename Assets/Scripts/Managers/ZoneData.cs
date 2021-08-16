using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneData
{
    public string currentZoneName;
    public int lastCheckPointIndex;
    public List<bool> switchStates;

    public ZoneData()
    {
        lastCheckPointIndex = LevelManager.lastCheckPoint.checkPointNumber;

        currentZoneName = GameManager.currentZoneName;

        switchStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
        {
            switchStates.Add(LevelManager.allZoneSwitchs[i].isOn);
        }
    }
}
