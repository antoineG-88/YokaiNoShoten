using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    Collider2D platformCollider;
    public float reactivationDelay;
    float timeRemaining;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.fixedDeltaTime;
        }

        platformCollider.enabled = timeRemaining <= 0;
    }

    private void Update()
    {
        if ((GameManager.isUsingController ? (ControlsManager.aimAndMovementSwitched ? Input.GetAxisRaw("RightStickV") : Input.GetAxisRaw("LeftStickV")) > 0.5f : Input.GetKey(ControlsManager.downKey)) || GameData.grappleHandler.isTracting)
        {
            timeRemaining = reactivationDelay;
        }
    }
}
