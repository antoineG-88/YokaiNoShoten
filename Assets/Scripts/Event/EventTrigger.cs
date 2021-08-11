using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : Switch
{
    public List<EventPart> eventParts;

    public int neededStoryStepToTrigger;
    public int maxStoryStepToTrigger;
    public int storyStepProgressionAtTheEnd;

    private bool isInEvent;
    private bool eventTriggered;

    private int currentEventPartIndex;
    private bool eventPartStarted;
    [HideInInspector] public CameraConstraintZone previousCamera;

    private new void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (isInEvent)
        {
            if(currentEventPartIndex < eventParts.Count)
            {
                if (!eventParts[currentEventPartIndex].eventStarted)
                {
                    eventParts[currentEventPartIndex].StartEventPart();
                    SetNewCameraConstraint();
                }
                else if(eventParts[currentEventPartIndex].eventEnded)
                {
                    previousCamera = eventParts[currentEventPartIndex].cameraConstraintDuringEventPart;
                    currentEventPartIndex++;
                }
            }
            else
            {
                EndEvent();
            }

        }
    }

    private void StartEvent()
    {
        if(!eventTriggered && GameManager.currentStoryStep >= neededStoryStepToTrigger && GameManager.currentStoryStep <= maxStoryStepToTrigger)
        {
            for (int i = 0; i < eventParts.Count; i++)
            {
                eventParts[i].ResetEvent(this);
            }

            isInEvent = true;
            eventTriggered = true;
            currentEventPartIndex = 0;
            GameData.playerManager.inControl = false;
            previousCamera = null;
        }
    }

    private void EndEvent()
    {
        if(previousCamera != null)
        {
            GameData.cameraHandler.constraintZones.Remove(previousCamera);
        }
        GameData.playerManager.inControl = true;
        isInEvent = false;
        isOn = true;
    }


    public void SetNewCameraConstraint()
    {
        if (eventParts[currentEventPartIndex].cameraConstraintDuringEventPart != null && previousCamera != eventParts[currentEventPartIndex].cameraConstraintDuringEventPart)
        {
            GameData.cameraHandler.constraintZones.Remove(previousCamera);
            GameData.cameraHandler.constraintZones.Add(eventParts[currentEventPartIndex].cameraConstraintDuringEventPart);
        }
    }

    public void RemoveCurrentCamera()
    {
        if(eventParts[currentEventPartIndex].cameraConstraintDuringEventPart != null)
        {
            GameData.cameraHandler.constraintZones.Remove(eventParts[currentEventPartIndex].cameraConstraintDuringEventPart);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            StartEvent();
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }
}
