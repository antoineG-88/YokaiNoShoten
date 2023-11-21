using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeikiMoveEvent : EventPart
{
    public Vector2 seikiPositionToReach;
    public bool seikiOrientationIsRight;
    public float seikiWalkingSpeed;

    void Update()
    {
        if (eventStarted && !eventEnded)
        {

            if (Mathf.Abs(GameData.player.transform.position.x - seikiPositionToReach.x) < 5 * seikiWalkingSpeed * Time.fixedDeltaTime)
            {
                EndEventPart();
            }
            else
            {
                if (GameData.player.transform.position.x > seikiPositionToReach.x)
                {
                    GameData.movementHandler.noControlTargetSpeed = seikiWalkingSpeed * -1;
                }
                else
                {
                    GameData.movementHandler.noControlTargetSpeed = seikiWalkingSpeed;
                }
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
    }

    public override void EndEventPart()
    {
        GameData.movementHandler.noControlTargetSpeed = 0;
        GameData.movementHandler.horizontalTargetSpeed = 0;
        GameData.playerVisuals.facingRight = seikiOrientationIsRight;
        base.EndEventPart();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(seikiPositionToReach, Vector3.one * 0.3f);
    }
}
