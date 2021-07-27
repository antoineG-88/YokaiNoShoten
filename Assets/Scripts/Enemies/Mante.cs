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
    [Header("Cythe Attack settings")]
    public float cytheAttackCooldown;
    public float cytheAttackTeleguidingTime;
    public float cytheTimeBeforeSpin;
    public float cytheSpinRadius;
    public GameObject cythe;
    public float cytheMaxSpeed;
    public float cytheAccelerationForce;
    public float cytheDistanceSpeedDampeningRatio;
    public float cytheTargetPlayerOffsetDistance;
    public float cytheKnockbackDistance;
    public LayerMask playerMask;

    private bool isFleeing;
    private float distToFirstPortal;
    private float distToSecondPortal;
    private bool destinationReached;
    private float cytheCDElapsed;
    private float teleguidingTimeElapsed;
    private bool isTeleguidingCythe;
    private float cytheCurrentSpeed;
    private float cytheCurrentMaxSpeed;
    private Vector2 cytheTargetPos;
    private Vector2 cytheTargetDirection;
    private ContactFilter2D playerFilter;
    private bool isCytheComingBack;
    private bool isCytheSpinning;

    protected new void Start()
    {
        base.Start();
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(playerMask);
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

        if(inControl)
        {
            if (isFleeing)
            {
                if (Vector2.Distance((Vector2)transform.position - playerDirection, initialPos) < movementZoneRadius)
                {
                    targetPathfindingPosition = (Vector2)transform.position - playerDirection;
                }
            }
            else
            {
                targetPathfindingPosition = transform.position;
            }

            if(isTeleguidingCythe)
            {
                if (teleguidingTimeElapsed > cytheAttackTeleguidingTime)
                {
                    StartCoroutine(SpinCythe());
                }
                else
                {
                    teleguidingTimeElapsed += Time.deltaTime;

                    cytheTargetPos = (Vector2)GameData.player.transform.position - playerDirection * cytheTargetPlayerOffsetDistance;

                    cytheTargetDirection = cytheTargetPos - (Vector2)cythe.transform.position;
                    cytheTargetDirection.Normalize();

                    cytheCurrentMaxSpeed = Mathf.Clamp(Vector2.Distance(cythe.transform.position, cytheTargetPos) * cytheDistanceSpeedDampeningRatio, 0, cytheMaxSpeed);
                    cytheCurrentSpeed += Time.deltaTime * cytheAccelerationForce;
                    cytheCurrentSpeed = Mathf.Clamp(cytheCurrentSpeed, 0, cytheCurrentMaxSpeed);

                    if(Vector2.Distance(cythe.transform.position, cytheTargetPos) > 0.1f)
                    {
                        cythe.transform.position = (Vector2)cythe.transform.position + cytheCurrentSpeed * cytheTargetDirection * Time.deltaTime;
                    }
                }
            }
            else
            {
                if(isCytheComingBack)
                {
                    cytheTargetPos = transform.position;

                    cytheTargetDirection = cytheTargetPos - (Vector2)cythe.transform.position;
                    cytheTargetDirection.Normalize();
                    cytheCurrentSpeed += Time.deltaTime * cytheAccelerationForce;
                    cytheCurrentSpeed = Mathf.Clamp(cytheCurrentSpeed, 0, cytheMaxSpeed);

                    if (Vector2.Distance(cythe.transform.position, cytheTargetPos) > 0.1f)
                    {
                        cythe.transform.position = (Vector2)cythe.transform.position + cytheCurrentSpeed * cytheTargetDirection * Time.deltaTime;
                    }
                    else
                    {
                        isCytheComingBack = false;
                        cythe.SetActive(false);
                    }
                }
                else
                {
                    if (cytheCDElapsed > cytheAttackCooldown)
                    {
                        ThrowCythe();
                    }
                    else
                    {
                        cytheCDElapsed += Time.deltaTime;
                        teleguidingTimeElapsed = 0;
                    }
                }
            }

            CheckRetreat();
        }
    }

    private void ThrowCythe()
    {
        cythe.SetActive(true);
        isTeleguidingCythe = true;
        teleguidingTimeElapsed = 0;
        cytheCurrentSpeed = 0;
        cythe.transform.position = transform.position;
    }

    private IEnumerator SpinCythe()
    {
        cytheCDElapsed = 0;
        isCytheSpinning = true;
        isTeleguidingCythe = false;
        yield return new WaitForSeconds(cytheTimeBeforeSpin);
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCircle(cythe.transform.position, cytheSpinRadius, playerFilter, colliders);
        if(colliders.Count > 0)
        {
            cytheTargetDirection = (Vector2)GameData.player.transform.position - (Vector2)cythe.transform.position;
            cytheTargetDirection.Normalize();

            GameData.playerManager.TakeDamage(1, cytheTargetDirection * cytheKnockbackDistance);
        }
        yield return new WaitForSeconds(0.3f);
        isCytheComingBack = true;
        isCytheSpinning = false;
    }

    private void CheckRetreat()
    {
        if(!isTeleguidingCythe && !isCytheComingBack && !isCytheSpinning)
        {
            if (distToPlayer < distanceToRetreat)
            {
                distToFirstPortal = Vector2.Distance(firstPortal.transform.position, transform.position);
                distToSecondPortal = Vector2.Distance(secondPortal.transform.position, transform.position);

                if (distToFirstPortal > distToSecondPortal)
                {
                    StartCoroutine(Retreat(firstPortal));
                }
                else
                {
                    StartCoroutine(Retreat(secondPortal));
                }
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
