using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraintZone : MonoBehaviour
{
    [Header("Camera Constraints")]
    public Vector2 cameraBaseOffset;
    public bool useAsCameraFocusPoint;
    public float cameraSize;
    public bool limitRelativeToZonePos;
    public float upLimit;
    public float downLimit;
    public float leftLimit;
    public float rightLimit;
    public Color cameraPreviewColor;
    public bool isTriggeredExternaly;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && !isTriggeredExternaly)
        {
            GameData.cameraHandler.constraintZones.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !isTriggeredExternaly)
        {
            GameData.cameraHandler.constraintZones.Remove(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 basePos = (Vector2)transform.position + cameraBaseOffset;
        if(useAsCameraFocusPoint)
        {
            basePos = cameraBaseOffset;
        }

        Gizmos.color = cameraPreviewColor;
        Gizmos.DrawCube(basePos, new Vector2(cameraSize * 2 * 16 / 9, cameraSize * 2));

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position, new Vector2(0.8f, 1.2f));

        if(upLimit != 0)
        {
            Gizmos.color = Color.red;
            if(limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(-1000, upLimit), (Vector2)transform.position + new Vector2(1000, upLimit));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(-1000, upLimit), new Vector2(1000, upLimit));
            }

            Gizmos.color = Color.yellow;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(-1000, upLimit - 0.5f), (Vector2)transform.position + new Vector2(1000, upLimit - 0.5f));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(-1000, upLimit - 0.5f), new Vector2(1000, upLimit - 0.5f));
            }

            if(upLimit < downLimit)
            {
                upLimit = 0;
            }
        }

        if (downLimit != 0)
        {
            Gizmos.color = Color.red;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(-1000, downLimit), (Vector2)transform.position + new Vector2(1000, downLimit));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(-1000, downLimit), new Vector2(1000, downLimit));
            }

            Gizmos.color = Color.yellow;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(-1000, downLimit + 0.5f), (Vector2)transform.position + new Vector2(1000, downLimit + 0.5f));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(-1000, downLimit + 0.5f), new Vector2(1000, downLimit + 0.5f));
            }
        }

        if (leftLimit != 0)
        {
            Gizmos.color = Color.red;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(leftLimit, -1000), (Vector2)transform.position + new Vector2(leftLimit, 1000));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(leftLimit, -1000), new Vector2(leftLimit, 1000));
            }

            Gizmos.color = Color.yellow;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(leftLimit + 0.5f, -1000), (Vector2)transform.position + new Vector2(leftLimit + 0.5f, 1000));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(leftLimit + 0.5f, -1000), new Vector2(leftLimit + 0.5f, 1000));
            }
        }

        if (rightLimit != 0)
        {
            Gizmos.color = Color.red;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(rightLimit, -1000), (Vector2)transform.position + new Vector2(rightLimit, 1000));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(rightLimit, -1000), new Vector2(rightLimit, 1000));
            }

            Gizmos.color = Color.yellow;
            if (limitRelativeToZonePos)
            {
                Gizmos.DrawLine((Vector2)transform.position + new Vector2(rightLimit - 0.5f, -1000), (Vector2)transform.position + new Vector2(rightLimit - 0.5f, 1000));
            }
            else
            {
                Gizmos.DrawLine(new Vector2(rightLimit - 0.5f, -1000), new Vector2(rightLimit - 0.5f, 1000));
            }

            if (rightLimit < leftLimit)
            {
                rightLimit = 0;
            }
        }
    }
}
