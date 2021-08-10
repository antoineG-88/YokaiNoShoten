using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpent : Enemy
{
    [Header("Movement settings")]
    public float maxSpeed;
    public float fleeMaxSpeed;
    public float noGravityZoneMaxSpeed;
    public float accelerationForce;
    public float wallDetectionDistance;
    public float slowSpeed;
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
    [Header("Display settings")]
    public List<Rigidbody2D> bodiesRb;
    public SpriteRenderer headSprite;
    public float maxDistanceBetweenBodyParts;
    public float bounceCircleRadiusTest;

    private bool isTooFarFromPlayer;
    private float currentSpeed;
    private Vector2 currentDirection;
    private float currentAngle;
    private float targetSpeed;
    private float angleDifferenceToTarget;
    private bool wallAhead;
    private List<Vector2> movementTrailPos;
    private List<int> currentBodyTrailPosIndex;

    private ContactFilter2D playerFilter;
    private bool isSpikesActive;
    private float inactiveSpikeTimeElapsed;
    private Vector2 detectionZoneCenterOffset;
    private bool headingToFirstPoint;
    private bool isFacingLeft;
    private List<SpriteRenderer> bodiesSprite;
    [HideInInspector] public bool isInNoGravityZone;

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
        bodiesSprite = new List<SpriteRenderer>();
        currentBodyTrailPosIndex = new List<int>();
        for (int i = 0; i < bodiesRb.Count; i++)
        {
            bodiesSprite.Add(bodiesRb[i].GetComponentInChildren<SpriteRenderer>());
            currentBodyTrailPosIndex.Add(0);
        }
        movementTrailPos = new List<Vector2>();
        movementTrailPos.Add(transform.position);
    }

    private new void Update()
    {
        base.Update();
        UpdateVisuals();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateSpikes();
    }

    public override void UpdateMovement()
    {
        wallAhead = Physics2D.Raycast(transform.position, currentDirection, wallDetectionDistance, LayerMask.GetMask("Wall"));
        targetSpeed = isInNoGravityZone ? noGravityZoneMaxSpeed : (wallAhead ? slowSpeed : (isSpikesActive ? maxSpeed : fleeMaxSpeed));

        if (!inControl)
        {
            currentSpeed = rb.velocity.magnitude;
            currentDirection = rb.velocity.normalized;
        }

        if(Physics2D.OverlapCircle(transform.position, bounceCircleRadiusTest, LayerMask.GetMask("Wall")))
        {
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, currentDirection, bounceCircleRadiusTest * 2, LayerMask.GetMask("Wall"));
            if(wallHit)
            {
                currentDirection = Vector2.Reflect(currentDirection, wallHit.normal);
            }
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

        if(!isInNoGravityZone)
        {
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
        }

        rb.velocity = currentDirection * currentSpeed;
        movementTrailPos.Add(transform.position);
        for (int i = 0; i < currentBodyTrailPosIndex.Count; i++)
        {
            //currentBodyTrailPosIndex[i]++;
        }
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
        isFacingLeft = currentDirection.x < 0;

        UpdateBodyPos();
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        isTooFarFromPlayer = distToPlayer > rangeGoalToPlayer;
        //provoked = Vector2.Distance(initialPos, GameData.player.transform.position) < provocationRange;

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

    private void UpdateBodyPos()
    {
        for (int i = 0; i < bodiesRb.Count; i++)
        {
            while (Vector2.Distance(movementTrailPos[currentBodyTrailPosIndex[i]], i == 0 ? (Vector2)transform.position : movementTrailPos[currentBodyTrailPosIndex[i - 1]]) > maxDistanceBetweenBodyParts)
            {
                currentBodyTrailPosIndex[i]++;
            }
            bodiesRb[i].transform.position = movementTrailPos[currentBodyTrailPosIndex[i]];
            Vector2 direction = movementTrailPos[currentBodyTrailPosIndex[i]] - bodiesRb[i].position;
            bodiesRb[i].velocity = direction;

            while(i == (bodiesRb.Count - 1) && currentBodyTrailPosIndex[i] > 0)
            {
                movementTrailPos.RemoveAt(0);
                for (int y = 0; y < currentBodyTrailPosIndex.Count; y++)
                {
                    currentBodyTrailPosIndex[y]--;
                }
            }
        }

        for (int i = 0; i < movementTrailPos.Count; i++)
        {
            Debug.DrawRay(movementTrailPos[i], Vector3.up * 0.1f, Color.blue);
        }
    }

    public override void DamageEffect()
    {
        maxSpeed = 0;
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void UpdateVisuals()
    {
        if (isFacingLeft && !headSprite.flipY)
        {
            headSprite.flipY = true;
        }
        if (!isFacingLeft && headSprite.flipY)
        {
            headSprite.flipY = false;
        }

        for (int i = 0; i < bodiesRb.Count; i++)
        {
            bodiesRb[i].transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, bodiesRb[i].velocity));

            if (bodiesRb[i].velocity.x < 0 && !bodiesSprite[i].flipY)
            {
                bodiesSprite[i].flipY = true;
            }
            if (bodiesRb[i].velocity.x > 0 && bodiesSprite[i].flipY)
            {
                bodiesSprite[i].flipY = false;
            }
        }
    }
}
