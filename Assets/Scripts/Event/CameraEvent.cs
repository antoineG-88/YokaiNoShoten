using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvent : EventPart
{
    public float focusTime;
    public float postFocusTime;

    private float timeElasped;
    private int currentFocusStep;

    private void Update()
    {
        if(eventStarted && !eventEnded)
        {
            if(timeElasped < (currentFocusStep == 0 ? focusTime : postFocusTime))
            {
                timeElasped += Time.deltaTime;
            }
            else
            {
                if(currentFocusStep == 1)
                {
                }
                else
                {
                    timeElasped = 0;
                    currentFocusStep = 1;
                    eventTrigger.RemoveCurrentCamera();
                    EndEventPart();
                }
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        timeElasped = 0;
        currentFocusStep = 0;
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
    }
}
