using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEvent : EventPart
{
    public Dialog dialogToPlay;
    //public CameraConstraintZone cameraConstraintDuringDialog;
    public Vector2 seikiPositionDuringDialog;
    public bool seikiOrientationIsRight;
    public float seikiWalkingSpeed;
    [Header("Technical Settings")]
    public GameObject seikiPosPreview;

    [HideInInspector] public bool isReachingDialoguePos;

    private DialogManager.EndDialCallback callback;

    private void Start()
    {
        seikiPosPreview.SetActive(false);
    }

    private void Update()
    {
        if(eventStarted && !eventEnded)
        {
            if (isReachingDialoguePos)
            {
                if (Mathf.Abs(GameData.movementHandler.transform.position.x - seikiPositionDuringDialog.x) < 20* seikiWalkingSpeed*Time.deltaTime)
                {
                    isReachingDialoguePos = false;
                    GameData.dialogManager.StartDialogue(dialogToPlay, callback);
                }

                if (GameData.movementHandler.transform.position.x > seikiPositionDuringDialog.x)
                {
                    GameData.movementHandler.noControlTargetSpeed = seikiWalkingSpeed * -1;
                }
                else
                {
                    GameData.movementHandler.noControlTargetSpeed = seikiWalkingSpeed;
                }
            }
            else
            {
                GameData.movementHandler.noControlTargetSpeed = 0;
                GameData.playerVisuals.facingRight = seikiOrientationIsRight;
            }
        }
    }

    public override void EndEventPart()
    {
        base.EndEventPart();
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        isReachingDialoguePos = true;
        callback = EndEventPart;
    }

    private void OnDrawGizmosSelected()
    {
        seikiPositionDuringDialog = seikiPosPreview.transform.position;
        //seikiOrientationIsRight = seikiPosPreview.transform.localScale.x == 1;
    }
}
