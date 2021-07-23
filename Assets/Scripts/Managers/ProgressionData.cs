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
        lastCheckPointIndex = LevelManager.lastCheckPoint.checkPointNumber;

        currentZoneName = GameManager.currentZoneName;

        switchStates = new List<bool>();
        for (int i = 0; i < LevelManager.allZoneSwitchs.Count; i++)
        {
            switchStates.Add(LevelManager.allZoneSwitchs[i].isOn);
        }

        currentStoryStep = GameManager.currentStoryStep;
    }
}
