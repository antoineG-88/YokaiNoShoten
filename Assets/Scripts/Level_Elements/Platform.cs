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
            platformCollider.enabled = false;
            timeRemaining -= Time.fixedDeltaTime;
        }
        else
        {
            platformCollider.enabled = true;
        }


        if (Input.GetAxisRaw("LeftStickV") == 1 || GameData.grappleHandler.isTracting)
        {
            timeRemaining = reactivationDelay;
        }
    }
}
