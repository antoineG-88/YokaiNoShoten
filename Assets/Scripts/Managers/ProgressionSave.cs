using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionSave
{
    public int chapterReached;
    public bool hasFinishedTheGame;
    public float fastestClearTime;
    public float[] chaptersClearTime;

    public ProgressionSave(int chapter)
    {
        chapterReached = chapter;
        fastestClearTime = 0f;
        hasFinishedTheGame = false;
        chaptersClearTime = new float[7] {0,0,0,0,0,0,0};
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

    public void UpdateChapterClearTime(int chapter, float clearTime)
    {
        if (chaptersClearTime[chapter - 1] != 0f)
        {
            chaptersClearTime[chapter - 1] = Mathf.Min(clearTime, chaptersClearTime[chapter - 1]);
        }
        else
        {
            chaptersClearTime[chapter - 1] = clearTime;
        }
    }
}
