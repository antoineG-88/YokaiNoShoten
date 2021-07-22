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
        for (int i = 0; i < GameManager.allZoneCheckPoints.Count; i++)
        {
            if(GameManager.lastCheckPoint == GameManager.allZoneCheckPoints[i])
            {
                lastCheckPointIndex = i;
            }
        }

        currentZoneName = GameManager.currentZoneName;

        switchStates = new List<bool>();
        for (int i = 0; i < GameManager.allZoneSwitchs.Count; i++)
        {
            switchStates.Add(GameManager.allZoneSwitchs[i].isOn);
        }

        currentStoryStep = GameManager.currentStoryStep;
    }
}
