using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionSave
{
    public int maxStoryStepReached;
    public bool hasFinishedTheGame;
    public float fastestClearTime;

    public ProgressionSave(int storyStep)
    {
        maxStoryStepReached = storyStep;
        fastestClearTime = 0f;
        hasFinishedTheGame = false;
    }

    public void UpdateProgression(int storyStep, bool finishedGame, float clearTime)
    {
        maxStoryStepReached = Mathf.Max(storyStep, maxStoryStepReached);
        if(finishedGame)
        {
            hasFinishedTheGame = true;
            fastestClearTime = Mathf.Min(clearTime, fastestClearTime);
        }
    }
}
