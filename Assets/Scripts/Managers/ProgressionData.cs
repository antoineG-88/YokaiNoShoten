using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionData
{
    public int currentStoryStep;

    public ProgressionData(int storyStep)
    {
        currentStoryStep = storyStep;
    }
}
