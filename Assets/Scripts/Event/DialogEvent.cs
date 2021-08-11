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
        seikiOrientationIsRight = seikiPosPreview.transform.localScale.x == 1;
    }
}
