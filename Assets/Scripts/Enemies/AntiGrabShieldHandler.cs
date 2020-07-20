using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGrabShieldHandler : MonoBehaviour
{
    public AntigrabShield[] shields;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        CheckGrappleRope();
    }

    private bool CheckGrappleRope()
    {
        bool antigrabTriggered = false;
        Debug.DrawRay(transform.position, -GameData.grappleHandler.tractionDirection, Color.red);
        if (GameData.grappleHandler.isHooked && GameData.grappleHandler.attachedObject == gameObject)
        {
            float tractionAngle = Vector2.SignedAngle(Vector2.right, -GameData.grappleHandler.tractionDirection);
            foreach (AntigrabShield shield in shields)
            {
                float minAngle = shield.angle - shield.width / 2;
                float maxAngle = shield.angle + shield.width / 2;
                if (IsAngleBetween(tractionAngle, minAngle, maxAngle))
                {
                    antigrabTriggered = true;
                }
            }

            if (antigrabTriggered)
            {
                GameData.grappleHandler.BreakRope("Shield used at : " + tractionAngle);
            }
        }

        return antigrabTriggered;
    }

    private bool IsAngleBetween(float angle, float minAngle, float maxAngle)
    {
        bool isBetween = false;
        if (angle > minAngle && angle < maxAngle)
        {
            isBetween = true;
        }

        if (angle - 360 > minAngle && angle - 360 < maxAngle)
        {
            isBetween = true;
        }

        if (angle + 360 > minAngle && angle + 360 < maxAngle)
        {
            isBetween = true;
        }

        return isBetween;
    }

    [System.Serializable]
    public class AntigrabShield
    {
        public float angle;
        public float width;
    }
}
