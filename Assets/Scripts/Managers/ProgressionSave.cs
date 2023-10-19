using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionSave
{
    public int chapterReached;
    public bool hasFinishedTheGame;
    public float fastestClearTime;

    public ProgressionSave(int chapter)
    {
        chapterReached = chapter;
        fastestClearTime = 0f;
        hasFinishedTheGame = false;
    }

    public void UpdateProgression(int chapter, bool finishedGame, float clearTime)
    {
        chapterReached = Mathf.Max(chapter, chapterReached);
        if(finishedGame)
        {
            hasFinishedTheGame = true;
            if(fastestClearTime != 0f)
            {
                fastestClearTime = Mathf.Min(clearTime, fastestClearTime);
            }
            else
            {
                fastestClearTime = clearTime;
            }
        }
    }
}
