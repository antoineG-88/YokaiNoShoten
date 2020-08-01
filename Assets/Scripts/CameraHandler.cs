using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Follow settings")]
    //public bool useWallAvoidance;
    public Vector2 followCenterOffset;
    [Range(0.0f, 1f)] public float baseLerpSpeed;
    //public float edgePointOffset;
    public float aimOffsetLength;
    public float momentumOffsetAmplitude;
    public float maxMomentumOffset;
    [Header("General settings")]
    public float baseOrthographicSize = 5.625f;
    public float sizeLerpSpeed;

    private Camera mainCamera;
    private Vector2 cameraTarget;
    private Vector2 cameraFinalPos;
    [HideInInspector] public bool followPlayer;
    private float currentOrthographicSize;
    private float currentLerpSpeed;

    void Start()
    {
        mainCamera = Camera.main;
        followPlayer = true;
        currentOrthographicSize = baseOrthographicSize;
        currentLerpSpeed = baseLerpSpeed;
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
            cameraTarget = (Vector2)GameData.player.transform.position + followCenterOffset + AimOffset(GameData.grappleHandler.aimDirection) + MomentumOffset();
            //cameraFinalPos = useWallAvoidance ? cameraTarget + OffsetForCamera(cameraTarget, rooms, roomWidth) : cameraTarget;
            cameraFinalPos = cameraTarget;

            currentOrthographicSize = baseOrthographicSize;
            currentLerpSpeed = baseLerpSpeed;
        }
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
}
