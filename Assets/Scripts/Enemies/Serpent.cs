using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpent : Enemy
{
    [Header("Movement settings")]
    public float maxSpeed;
    public float fleeMaxSpeed;
    public float accelerationForce;
    public float wallDetectionDistance;
    public float slowSpeed;
    public float provocationRange;
    public float rangeGoalToPlayer;
    public float steeringRatio;
    public float minAngleDiffToTurn;
    public Transform patrolPos1;
    public Transform patrolPos2;
    [Header("Spikes settings")]
    public GameObject spikeDisplay;
    public int spikesDamage;
    public float spikesKnockbackForce;
    public float spikesReactivationTime;
    public Collider2D headSpikesCollider;
    public SerpentTail tail;

    private bool isTooFarFromPlayer;
    private float currentSpeed;
    private Vector2 currentDirection;
    private float currentAngle;
    private float targetSpeed;
    private float angleDifferenceToTarget;
    private bool wallAhead;

    private ContactFilter2D playerFilter;
    private bool isSpikesActive;
    private float inactiveSpikeTimeElapsed;
    private Vector2 detectionZoneCenterOffset;
    private bool headingToFirstPoint;

    private new void Start()
    {
        base.Start();
        provoked = false;
        currentDirection = Vector2.up;
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        tail.serpent = this;
        isSpikesActive = true;
        headingToFirstPoint = true;
    }

    private new void Update()
    {
        base.Update();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateSpikes();
    }

    public override void UpdateMovement()
    {
        wallAhead = Physics2D.Raycast(transform.position, currentDirection, wallDetectionDistance, LayerMask.GetMask("Wall"));
        targetSpeed = wallAhead ? slowSpeed : isSpikesActive ? maxSpeed : fleeMaxSpeed;

        if (!inControl)
        {
            currentSpeed = rb.velocity.magnitude;
            currentDirection = rb.velocity.normalized;
        }


        if (Mathf.Abs(currentSpeed - targetSpeed) <= 0.01f)
        {
            currentSpeed = targetSpeed;
        }
        else if (currentSpeed < targetSpeed)
        {
            currentSpeed += accelerationForce * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed -= accelerationForce * Time.fixedDeltaTime;
        }

        currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection);
        if (targetPathfindingPosition != (Vector2)transform.position)
        {
            Vector2 targetDirection = targetPathfindingPosition - (Vector2)transform.position;
            angleDifferenceToTarget = Vector2.SignedAngle(currentDirection, IsLineOfViewClearBetween(transform.position, targetPathfindingPosition) ? targetDirection : (GetPathNextPosition(2) - (Vector2)transform.position));
            if (Mathf.Abs(angleDifferenceToTarget) > minAngleDiffToTurn)
            {
                currentAngle += Mathf.Sign(angleDifferenceToTarget) * steeringRatio * currentSpeed * Time.fixedDeltaTime;
                currentDirection = DirectionFromAngle(currentAngle);
            }
        }
        else if (wallAhead)
        {
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, DirectionFromAngle(currentAngle + 20), wallDetectionDistance + 1, LayerMask.GetMask("Wall"));
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, DirectionFromAngle(currentAngle - 20), wallDetectionDistance + 1, LayerMask.GetMask("Wall"));
            float addedSteerAngle = steeringRatio * 7 * Time.fixedDeltaTime;
            currentAngle += leftHit ? rightHit ? leftHit.distance > rightHit.distance ? addedSteerAngle : -addedSteerAngle : -addedSteerAngle : rightHit ? addedSteerAngle : -addedSteerAngle;
            currentDirection = DirectionFromAngle(currentAngle);
        }

        rb.velocity = currentDirection * currentSpeed;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        isTooFarFromPlayer = distToPlayer > rangeGoalToPlayer;
        provoked = Vector2.Distance(initialPos, GameData.player.transform.position) < provocationRange;

        if (provoked)
        {
            if(isSpikesActive)
            {
                if (isTooFarFromPlayer)
                {
                    targetPathfindingPosition = FindNearestSightSpot(5, rangeGoalToPlayer, false);
                }
                else
                {
                    targetPathfindingPosition = transform.position;
                }
            }
            else
            {
                targetPathfindingPosition = (Vector2)transform.position - playerDirection * 3;
            }
        }
        else
        {
            if(headingToFirstPoint)
            {
                targetPathfindingPosition = patrolPos1.position;
                if(Vector2.Distance(transform.position, patrolPos1.position) < rangeGoalToPlayer)
                {
                    headingToFirstPoint = false;
                }
            }
            else
            {
                targetPathfindingPosition = patrolPos2.position;
                if (Vector2.Distance(transform.position, patrolPos2.position) < rangeGoalToPlayer)
                {
                    headingToFirstPoint = true;
                }
            }
            //targetPathfindingPosition = initialPos;
        }
    }

    private void UpdateSpikes()
    {
        if(isSpikesActive)
        {
            spikeDisplay.SetActive(true);
            List<Collider2D> colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(headSpikesCollider, playerFilter, colliders);
            if (colliders.Count > 0)
            {
                GameData.playerManager.TakeDamage(spikesDamage, playerDirection * spikesKnockbackForce);
                currentSpeed = 0;
            }
        }
        else
        {
            if(inactiveSpikeTimeElapsed >= spikesReactivationTime)
            {
                isSpikesActive = true;
            }
            inactiveSpikeTimeElapsed += Time.fixedDeltaTime;
            spikeDisplay.SetActive(false);
        }
        isProtected = isSpikesActive;
    }

    public void DisableSpikes()
    {
        isSpikesActive = false;
        inactiveSpikeTimeElapsed = 0;
        //ajouter anim et effet
    }

    public override void DamageEffect()
    {
        maxSpeed = 0;
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
