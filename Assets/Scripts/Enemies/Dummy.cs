using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    [Header("Movement settings")]
    public float acceleration;
    public float maxSpeed;
    public float slowingForce;
    public float stopDistance;

    private new void Start()
    {
        base.Start();
        provoked = true;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if (distToPlayer > stopDistance)
        {
            if (provoked)
            {
                targetPathfindingPosition = GameData.player.transform.position;
            }


            if (path != null && !pathEndReached)
            {
                Vector2 addedVelocity = new Vector2(pathDirection.x * acceleration, pathDirection.y * acceleration);

                rb.velocity += addedVelocity * Time.fixedDeltaTime;

                if (addedVelocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }
        }
        else
        {
            if (rb.velocity.magnitude > slowingForce * Time.fixedDeltaTime)
            {
                rb.velocity -= rb.velocity * slowingForce * Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public override void DamageEffect()
    {

    }
}
