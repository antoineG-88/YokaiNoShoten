using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTransition : EventPart
{
    public int nextZoneBuildIndex;
    public int nextZoneChapterNumber;
    public float seikiWalkingSpeed;
    public bool seikiWalkingRight;
    public float timeToTriggerLoad;
    public bool loadWithoutSaving;

    private float timeElapsed;
    private bool loadFlag;

    private void Update()
    {
        if (eventStarted)
        {
            GameData.movementHandler.noControlTargetSpeed = (seikiWalkingRight ? 1 : -1) * seikiWalkingSpeed;

            if(timeElapsed < timeToTriggerLoad)
            {
                timeElapsed += Time.deltaTime;
            }
            else if(loadFlag)
            {
                if(loadWithoutSaving)
                {
                    StartCoroutine(GameManager.LoadWithProgress(nextZoneBuildIndex));
                }
                else
                {
                    StartCoroutine(GameData.levelManager.LoadNewZone(nextZoneBuildIndex, nextZoneChapterNumber));
                }
                loadFlag = false;
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        loadFlag = true;
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
    }
}
