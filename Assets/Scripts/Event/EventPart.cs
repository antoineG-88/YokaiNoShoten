using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventPart : MonoBehaviour
{
    public CameraConstraintZone cameraConstraintDuringEventPart;

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
    }

    public virtual void EndEventPart()
    {
        eventEnded = true;
    }

}
