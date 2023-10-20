using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FinishGameEvent : EventPart
{
    public int storyStepToSave;
    public GameObject endgameScreen;
    public Text clearTime;
    public Text deathCount;
    public GameObject continueButton;

    public override void StartEventPart()
    {
        base.StartEventPart();
        SaveFinishedGameStats();
    }

    public void Update()
    {
        if(eventStarted)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f)
                {
                    EventSystem.current.SetSelectedGameObject(continueButton);
                }
            }
        }
    }

    public void SaveFinishedGameStats()
    {
        GameManager.currentStoryStep = storyStepToSave;
        SaveSystem.SaveGameAndProgression(GameManager.currentZoneName);

        if(GameManager.isValidForClearTime)
        {
            SaveSystem.SaveNewFinishedGame(GameManager.timeElapsedPlaying);
            ShowGameStats();
        }
        else
        {
            ContinueEnd();
        }
    }

    public void ShowGameStats()
    {
        GameSave finishSave = SaveSystem.LoadGameSave();
        float playTime = finishSave.timeElapsed;
        endgameScreen.SetActive(true);
        clearTime.text = (GameManager.GetHourFromSecondElapsed(playTime) == 0 ? "" : (GameManager.GetHourFromSecondElapsed(playTime) + "hours - "))
            + GameManager.GetMinutesFromSecondElapsed(playTime) + "min - "
            + (GameManager.GetSecondsFromSecondElapsed(playTime) + GameManager.GetSubSecondFromSecondElapsed(playTime)).ToString("0.00") + " seconds";
        deathCount.text = finishSave.numberOfDeath.ToString();

        EventSystem.current.SetSelectedGameObject(continueButton);
    }

    public void ContinueEnd()
    {
        SaveSystem.DeleteGameSaveFile();
        EndEventPart();
    }
}