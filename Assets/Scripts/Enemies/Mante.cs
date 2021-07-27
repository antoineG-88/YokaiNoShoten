using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mante : Enemy
{
    [Header("Mante settings")]
    public float distanceToRetreat;
    public float fleeSpeed;
    public float maxDistanceToFlee;
    public GameObject firstPortal;
    public GameObject secondPortal;
    public float accelerationForce;
    public float stopDistance;

    private bool isFleeing;
    private float distToFirstPortal;
    private float distToSecondPortal;
    private bool destinationReached;
    private float attackCDElapsed;
    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        base.Update();
    }
    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        isFleeing = distToPlayer < maxDistanceToFlee;
        destinationReached = Vector2.Distance(transform.position, targetPathfindingPosition) < stopDistance;

        if(isFleeing)
        {
            targetPathfindingPosition = (Vector2)transform.position - playerDirection * 2;
        }
        else
        {
            targetPathfindingPosition = transform.position;
        }

        CheckRetreat();
    }

    private void CheckRetreat()
    {
        if(distToPlayer < distanceToRetreat)
        {
            distToFirstPortal = Vector2.Distance(firstPortal.transform.position, transform.position);
            distToSecondPortal = Vector2.Distance(secondPortal.transform.position, transform.position);

            if(distToFirstPortal > distToSecondPortal)
            {
                StartCoroutine(Retreat(firstPortal));
            }
            else
            {
                StartCoroutine(Retreat(secondPortal));
            }
        }
    }

    private IEnumerator Retreat(GameObject portal)
    {
        if (GameData.grappleHandler.attachedObject == gameObject)
        {
            GameData.grappleHandler.BreakRope("Mante tp");
        }
        transform.position = portal.transform.position;
        yield return null;
    }

    public override void UpdateMovement()
    {
        if (path != null && !pathEndReached && inControl)
        {
            Vector2 force = new Vector2(pathDirection.x * accelerationForce, pathDirection.y * accelerationForce);

            rb.velocity += force * Time.fixedDeltaTime;

            if (rb.velocity.magnitude > fleeSpeed)
            {
                rb.velocity = rb.velocity.normalized * fleeSpeed;
            }
        }
        else if (destinationReached)
        {
            rb.velocity -= rb.velocity.normalized * accelerationForce * Time.fixedDeltaTime;
            if (rb.velocity.magnitude <= accelerationForce * Time.fixedDeltaTime)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public override void DamageEffect()
    {

    }

}
