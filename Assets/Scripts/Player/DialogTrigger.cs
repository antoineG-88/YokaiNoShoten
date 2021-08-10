using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogTrigger : Switch
{
    public Dialog dialogToPlay;
    public CameraConstraintZone cameraConstraintDuringDialog;
    public Vector2 seikiPositionDuringDialog;
    public bool seikiOrientationIsRight;
    public float seikiWalkingSpeed;
    [Header("Technical Settings")]
    public GameObject seikiPosPreview;

    private bool eventTriggered;
    private DialogManager.EndDialCallback callback;
    private bool isReachingDialoguePos;
    private bool isInEvent;

    private new void Start()
    {
        base.Start();
        seikiPosPreview.SetActive(false);
    }

    private void Update()
    {
        if(isInEvent)
        {
            if (isReachingDialoguePos)
            {
                if (Vector2.Distance(GameData.movementHandler.transform.position, seikiPositionDuringDialog) < 0.1f)
                {
                    isReachingDialoguePos = false;
                    GameData.dialogManager.StartDialogue(dialogToPlay, callback);
                }

                if (GameData.movementHandler.transform.position.x > seikiPositionDuringDialog.x)
                {
                    GameData.movementHandler.horizontalTargetSpeed = seikiWalkingSpeed * -1;
                }
                else
                {
                    GameData.movementHandler.horizontalTargetSpeed = seikiWalkingSpeed;
                }
            }
            else
            {
                GameData.movementHandler.horizontalTargetSpeed = 0;
            }
        }
    }

    private void StartEvent()
    {
        if(!eventTriggered)
        {
            isInEvent = true;
            eventTriggered = true;
            isReachingDialoguePos = true;
            callback = EndEvent;
            GameData.cameraHandler.constraintZones.Add(cameraConstraintDuringDialog);
            GameData.playerManager.inControl = false;
        }
    }

    private void EndEvent()
    {
        GameData.cameraHandler.constraintZones.Remove(cameraConstraintDuringDialog);
        GameData.playerManager.inControl = true;
        isInEvent = false;
        isOn = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            StartEvent();
        }
    }

    private void OnDrawGizmosSelected()
    {
        seikiPositionDuringDialog = seikiPosPreview.transform.position;
        seikiOrientationIsRight = seikiPosPreview.transform.localScale.x == 1;
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }
}
