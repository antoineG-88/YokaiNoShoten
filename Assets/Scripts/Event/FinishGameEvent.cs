using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGameEvent : EventPart
{
    public int storyStepToSave;

    public override void StartEventPart()
    {
        base.StartEventPart();
        FinishGame();
    }

    public void FinishGame()
    {
        GameManager.currentStoryStep = storyStepToSave;
        SaveSystem.SaveGameAndProgression(GameManager.currentZoneName);
        EndEventPart();
    }

    public void ShowGameStats()
    {
        //Shwo time clear + deaths
    }
}