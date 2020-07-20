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

    public void CheckGrappleRope()
    {
        if (GameData.grappleHandler.isHooked && GameData.grappleHandler.attachedObject == gameObject)
        {
            if (!CanBeGrappledFrom(GameData.grappleHandler.tractionDirection))
            {
                GameData.grappleHandler.BreakRope();
            }
        }
    }

    public bool CanBeGrappledFrom(Vector2 grappleDirection)
    {
        bool antigrabTriggered = false;
        Debug.DrawRay(transform.position, -grappleDirection, Color.red);

        float tractionAngle = Vector2.SignedAngle(Vector2.right, -grappleDirection);
        foreach (AntigrabShield shield in shields)
        {
            float minAngle = shield.angle - shield.width / 2;
            float maxAngle = shield.angle + shield.width / 2;
            if (IsAngleBetween(tractionAngle, minAngle, maxAngle))
            {
                antigrabTriggered = true;
            }
        }

        return !antigrabTriggered;
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

    public void ChangeShieldAngle(AntigrabShield shield, float newAngle)
    {
        shield.angle = newAngle;
        shield.displayO.transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    [System.Serializable]
    public class AntigrabShield
    {
        public float angle;
        public float width;
        public GameObject displayO;
    }
}
