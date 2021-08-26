using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : Switch
{
    [Header("Add event component on this object and drop them in order in the list")]
    public List<EventPart> eventParts;
    [Header("Optionnal, put a switch if the event is triggered by a switch")]
    public Switch switchToTriggerEvent;


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
            GameData.playerManager.inControl = false;
            if (currentEventPartIndex < eventParts.Count)
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
        else
        {
            if(switchToTriggerEvent != null && switchToTriggerEvent.IsON() && !eventTriggered)
            {
                StartEvent();
            }
        }

    }

    private void StartEvent()
    {
        if(!eventTriggered && GameManager.currentStoryStep >= neededStoryStepToTrigger && GameManager.currentStoryStep <= maxStoryStepToTrigger && !isOn)
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
        if(storyStepProgressionAtTheEnd != 0)
            GameManager.currentStoryStep = storyStepProgressionAtTheEnd;
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
        if(collider.CompareTag("Player") && switchToTriggerEvent == null)
        {
            StartEvent();
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }
}
