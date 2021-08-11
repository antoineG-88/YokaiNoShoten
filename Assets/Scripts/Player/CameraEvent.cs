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
            if(timeElasped < focusTime)
            {
                timeElasped += Time.deltaTime;
            }
            else
            {
                if(currentFocusStep == 1)
                {
                    EndEventPart();
                }
                else
                {
                    timeElasped = 0;
                    currentFocusStep = 1;
                    eventTrigger.RemoveCurrentCamera();
                }
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        timeElasped = 0;
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
    }
}
