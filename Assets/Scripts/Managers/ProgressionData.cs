using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionData
{
    public int currentStoryStep;
    public string currentZoneName;
    public int lastCheckPointIndex;
    public List<bool> switchStates;

    public ProgressionData()
    {
        /*for (int i = 0; i < LevelManager.allZoneCheckPoints.Count; i++)
        {
            if(LevelManager.lastCheckPoint == LevelManager.allZoneCheckPoints[i])
            {
                lastCheckPointIndex = i;
            }
        }*/
        lastCheckPointIndex = LevelManager.lastCheckPoint.checkPointNumber;
        Debug.Log("index saved : " + lastCheckPointIndex);

        currentZoneName = GameManager.currentZoneName;

        switchStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
        {
            switchStates.Add(LevelManager.allZoneSwitchs[i].isOn);
        }

        currentStoryStep = GameManager.currentStoryStep;
    }
}
