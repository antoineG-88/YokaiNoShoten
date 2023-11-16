using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventPart : MonoBehaviour
{
    public CameraConstraintZone cameraConstraintDuringEventPart;
    public bool freezeTime;

    [HideInInspector] public EventTrigger eventTrigger;
    [HideInInspector] public bool eventStarted;
    [HideInInspector] public bool eventEnded;

    public void ResetEvent(EventTrigger connectedEvent)
    {
        eventEnded = false;
        eventStarted = false;
        eventTrigger = connectedEvent;
    }

    public virtual void StartEventPart()
    {
        eventStarted = true;
        if(freezeTime)
        {
            GameData.slowMoManager.FreezeTime();
        }
    }

    public virtual void EndEventPart()
    {
        eventEnded = true;
        if (freezeTime)
        {
            GameData.slowMoManager.UnFreezeTime();
        }
    }

}
