using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSave
{
    public ZoneSave lastZoneData;
    public int currentStoryStep;
    public bool isValidRun;
    public float timeElapsed;
    public int numberOfDeath;

    public GameSave()
    {
        lastZoneData = new ZoneSave();
        currentStoryStep = GameManager.currentStoryStep;
        timeElapsed = GameManager.timeElapsedPlaying;
        numberOfDeath = GameManager.numberOfDeath;
        isValidRun = GameManager.isValidForClearTime;
    }
}
