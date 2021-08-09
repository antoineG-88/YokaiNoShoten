using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Follow settings")]
    public Vector2 followCenterOffset;
    [Range(0.0f, 1f)] public float baseLerpSpeed;
    public float aimOffsetLength;
    public float momentumOffsetAmplitude;
    public float maxMomentumOffset;
    public bool additionnalOffsetConstraint;
    public float edgeMargin;
    [Header("General settings")]
    public float baseOrthographicSize = 5.625f;
    public float sizeLerpSpeed;

    private Camera mainCamera;
    private Vector2 cameraTarget;
    private Vector2 cameraFinalPos;
    [HideInInspector] public bool followPlayer;
    private float currentOrthographicSize;
    private float currentLerpSpeed;
    [HideInInspector] public List<CameraConstraintZone> constraintZones;

    void Start()
    {
        mainCamera = Camera.main;
        followPlayer = true;
        currentOrthographicSize = baseOrthographicSize;
        currentLerpSpeed = baseLerpSpeed;
        constraintZones = new List<CameraConstraintZone>();
    }

    void FixedUpdate()
    {
        UpdateCameraTarget();

        MoveCamera(cameraFinalPos);
    }

    private void MoveCamera(Vector2 targetCameraPos)
    {
        Vector2 lerpPos = Vector2.Lerp(mainCamera.transform.position, targetCameraPos, currentLerpSpeed * Time.fixedDeltaTime * 50);
        mainCamera.transform.position = new Vector3(lerpPos.x, lerpPos.y, -10.0f);

        if (Mathf.Abs(mainCamera.orthographicSize - currentOrthographicSize) > 0.01f)
        {
            mainCamera.orthographicSize -= (mainCamera.orthographicSize - currentOrthographicSize) * sizeLerpSpeed * Time.fixedDeltaTime;
        }
        else
        {
            mainCamera.orthographicSize = currentOrthographicSize;
        }
    }

    private void UpdateCameraTarget()
    {
        if (followPlayer)
        {
            if(constraintZones.Count > 0 && constraintZones[constraintZones.Count - 1].useAsCameraFocusPoint)
            {
                cameraTarget = constraintZones[constraintZones.Count - 1].cameraBaseOffset;
            }
            else
            {
                cameraTarget = (Vector2)GameData.player.transform.position + followCenterOffset + AimOffset(GameData.grappleHandler.aimDirection) + MomentumOffset() + ZoneOffset();
            }
            currentOrthographicSize = ZoneSize();

            cameraFinalPos = ValidPosByZoneLimits(cameraTarget);

            currentLerpSpeed = baseLerpSpeed;
        }
    }


    bool isOutUpEdge = false;
    bool isOutDownEdge = false;
    bool isOutRightEdge = false;
    bool isOutLeftEdge = false;
    CameraConstraintZone currentZone;
    Vector2 correctedCameraPos;
    Vector2 upEdgePos;
    Vector2 downEdgePos;
    Vector2 rightEdgePos;
    Vector2 leftEdgePos;
    private Vector2 ValidPosByZoneLimits(Vector2 originPos)
    {
        correctedCameraPos = originPos;
        
        upEdgePos = originPos + currentOrthographicSize * Vector2.up;
        downEdgePos = originPos + currentOrthographicSize * Vector2.down;
        rightEdgePos = originPos + (currentOrthographicSize * mainCamera.aspect) * Vector2.right;
        leftEdgePos = originPos + (currentOrthographicSize * mainCamera.aspect) * Vector2.left;

        /*Debug.DrawRay(upEdgePos, Vector2.down);
        Debug.DrawRay(downEdgePos, Vector2.up);
        Debug.DrawRay(rightEdgePos, Vector2.left);
        Debug.DrawRay(leftEdgePos, Vector2.right);
        Debug.DrawRay(originPos, Vector2.up * 1);*/

        if (constraintZones.Count > 0)
        {
            currentZone = constraintZones[constraintZones.Count - 1];
            float absoluteUpLimit = (currentZone.limitRelativeToZonePos ? currentZone.transform.position.y + currentZone.upLimit : currentZone.upLimit) + edgeMargin;
            float absoluteDownLimit = (currentZone.limitRelativeToZonePos ? currentZone.transform.position.y + currentZone.downLimit : currentZone.downLimit) - edgeMargin;
            float absoluteRightLimit = (currentZone.limitRelativeToZonePos ? currentZone.transform.position.x + currentZone.rightLimit : currentZone.rightLimit) + edgeMargin;
            float absoluteLeftLimit = (currentZone.limitRelativeToZonePos ? currentZone.transform.position.x + currentZone.leftLimit : currentZone.leftLimit) - edgeMargin;

            isOutUpEdge = false;
            if (upEdgePos.y > absoluteUpLimit && currentZone.upLimit != 0)
            {
                isOutUpEdge = true;
                correctedCameraPos = new Vector2(correctedCameraPos.x, absoluteUpLimit - currentOrthographicSize);
            }

            isOutDownEdge = false;
            if (downEdgePos.y < absoluteDownLimit && currentZone.downLimit != 0)
            {
                isOutDownEdge = true;
                correctedCameraPos = new Vector2(correctedCameraPos.x, absoluteDownLimit + currentOrthographicSize);
            }

            if(isOutUpEdge && isOutDownEdge)
            {
                correctedCameraPos = new Vector2(correctedCameraPos.x, (absoluteUpLimit + absoluteDownLimit) / 2);
            }




            isOutRightEdge = false;
            if (rightEdgePos.x > absoluteRightLimit && currentZone.rightLimit != 0)
            {
                isOutRightEdge = true;
                correctedCameraPos = new Vector2(absoluteRightLimit - currentOrthographicSize * mainCamera.aspect, correctedCameraPos.y);
            }

            isOutLeftEdge = false;
            if (leftEdgePos.x < absoluteLeftLimit && currentZone.leftLimit != 0)
            {
                isOutLeftEdge = true;
                correctedCameraPos = new Vector2(absoluteLeftLimit + currentOrthographicSize * mainCamera.aspect, correctedCameraPos.y);
            }

            if (isOutRightEdge && isOutLeftEdge)
            {
                correctedCameraPos = new Vector2((absoluteLeftLimit + absoluteRightLimit) / 2, correctedCameraPos.y);
            }
        }
        Debug.DrawRay(correctedCameraPos, Vector2.up, Color.green);
        return correctedCameraPos;
    }

    private Vector2 ZoneOffset()
    {
        Vector2 offset = Vector2.zero;
        if(constraintZones.Count > 0)
        {
            if (additionnalOffsetConstraint)
            {
                for (int i = 0; i < constraintZones.Count; i++)
                {
                    offset += constraintZones[i].cameraBaseOffset;
                }
                offset /= constraintZones.Count - 1;
            }
            else
            {
                offset = constraintZones[constraintZones.Count - 1].cameraBaseOffset;
            }
        }
        return offset;
    }

    private float ZoneSize()
    {
        float size = baseOrthographicSize;
        if (constraintZones.Count > 0)
        {
            size = constraintZones[constraintZones.Count - 1].cameraSize;
        }
        return size;
    }

    private Vector2 AimOffset(Vector2 aimDirection)
    {
        Vector2 offset = Vector2.zero;

        offset = new Vector2(aimDirection.x, aimDirection.y) * aimOffsetLength;

        return offset;
    }

    private Vector2 MomentumOffset()
    {
        Vector2 offset = GameData.movementHandler.rb.velocity * momentumOffsetAmplitude / 100;
        if (offset.magnitude > maxMomentumOffset)
        {
            offset = offset.normalized * maxMomentumOffset;
        }
        return offset;
    }

    public bool IsPointInCameraView(Vector2 positionTested, float margin)
    {
        bool isInView = true;

        if(positionTested.x > transform.position.x + (currentOrthographicSize * mainCamera.aspect) + margin)
        {
            isInView = false;
        }

        if (positionTested.x < transform.position.x - (currentOrthographicSize * mainCamera.aspect) - margin)
        {
            isInView = false;
        }

        if (positionTested.y > transform.position.y + (currentOrthographicSize) + margin)
        {
            isInView = false;
        }

        if (positionTested.y < transform.position.y - (currentOrthographicSize) - margin)
        {
            isInView = false;
        }

        return isInView;
    }
}
