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
        if ((GameData.playerManager.isUsingController ? Input.GetAxisRaw("LeftStickV") > 0.5f : Input.GetAxisRaw("Vertical") < -0.5f ) || GameData.grappleHandler.isTracting)
        {
            timeRemaining = reactivationDelay;
        }
    }
}
