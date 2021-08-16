using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTransition : EventPart
{
    public int nextZoneBuildIndex;
    public float seikiWalkingSpeed;
    public bool seikiWalkingRight;
    public float timeToTriggerLoad;

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
                StartCoroutine(GameData.levelManager.LoadNewZone(nextZoneBuildIndex));
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
