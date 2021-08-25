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
    public float noGravitySteeringRatio;
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
    public float bounceCircleRadiusTest;
    public float angleCorrection;
    public float headHeightCorrection;
    public int trailSubdivision;

    private bool isTooFarFromPlayer;
    private float currentSpeed;
    private Vector2 currentDirection;
    private float currentAngle;
    private float targetSpeed;
    private float angleDifferenceToTarget;
    private bool wallAhead;
    private List<Vector2> movementTrailPos;
    private List<int> currentBodyTrailPosIndex;
    private float[] maxDistanceBetweenBodyParts;

    private ContactFilter2D playerFilter;
    private bool isSpikesActive;
    private float inactiveSpikeTimeElapsed;
    private Vector2 detectionZoneCenterOffset;
    private bool headingToFirstPoint;
    private bool isFacingLeft;
    private List<SpriteRenderer> bodiesSprite;
    private List<Animator> bodiesAnimator;
    private Vector2 previousHeadPos;
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
        bodiesAnimator = new List<Animator>();
        currentBodyTrailPosIndex = new List<int>();
        for (int i = 0; i < bodiesRb.Count; i++)
        {
            bodiesSprite.Add(bodiesRb[i].GetComponentInChildren<SpriteRenderer>());
            bodiesAnimator.Add(bodiesRb[i].GetComponentInChildren<Animator>());
            currentBodyTrailPosIndex.Add(0);
        }
        movementTrailPos = new List<Vector2>();
        movementTrailPos.Add(transform.position);

        maxDistanceBetweenBodyParts = new float[bodiesRb.Count];
        for (int i = 0; i < bodiesRb.Count; i++)
        {
            if (i != 0)
            {
                maxDistanceBetweenBodyParts[i] = Vector2.Distance(bodiesRb[i - 1].transform.position, bodiesRb[i].transform.position);
            }
            else
            {
                maxDistanceBetweenBodyParts[i] = Vector2.Distance(transform.position, bodiesRb[i].transform.position);
            }
        }
        previousHeadPos = transform.position;
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

        if (Physics2D.OverlapCircle(transform.position, bounceCircleRadiusTest, LayerMask.GetMask("Wall", "DashWall", "EnemyProof")))
        {
            RaycastHit2D wallHit = Physics2D.CircleCast(transform.position, bounceCircleRadiusTest, currentDirection, bounceCircleRadiusTest * 2, LayerMask.GetMask("Wall", "DashWall", "EnemyProof"));
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

        currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection);
        if (targetPathfindingPosition != (Vector2)transform.position)
        {
            Vector2 targetDirection = targetPathfindingPosition - (Vector2)transform.position;
            angleDifferenceToTarget = Vector2.SignedAngle(currentDirection, IsLineOfViewClearBetween(transform.position, targetPathfindingPosition) ? targetDirection : (GetPathNextPosition(2) - (Vector2)transform.position));
            if (Mathf.Abs(angleDifferenceToTarget) > minAngleDiffToTurn)
            {
                currentAngle += Mathf.Sign(angleDifferenceToTarget) * (isInNoGravityZone ? noGravitySteeringRatio : steeringRatio) * currentSpeed * Time.fixedDeltaTime;
                currentDirection = DirectionFromAngle(currentAngle);
            }
        }
        else if (wallAhead)
        {
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, DirectionFromAngle(currentAngle + 20), wallDetectionDistance + 1, LayerMask.GetMask("Wall"));
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, DirectionFromAngle(currentAngle - 20), wallDetectionDistance + 1, LayerMask.GetMask("Wall"));
            float addedSteerAngle = (isInNoGravityZone ? noGravitySteeringRatio : steeringRatio) * 7 * Time.fixedDeltaTime;
            currentAngle += leftHit ? rightHit ? leftHit.distance > rightHit.distance ? addedSteerAngle : -addedSteerAngle : -addedSteerAngle : rightHit ? addedSteerAngle : -addedSteerAngle;
            currentDirection = DirectionFromAngle(currentAngle);
        }

        if (!isDying)
        {
            rb.velocity = currentDirection * currentSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        for (int i = 0; i < 10; i++)
        {
            movementTrailPos.Add(Vector2.Lerp(previousHeadPos, transform.position, (float)(i + 1)/10));
        }

        if(!isDying)
        {
            previousHeadPos = transform.position;

            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
            isFacingLeft = currentDirection.x < 0;

            UpdateBodyPos();
        }
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
                GameData.pierceHandler.isPiercing = false;
                GameData.playerManager.TakeDamage(spikesDamage, playerDirection * spikesKnockbackForce);
                currentSpeed = 0;
            }
            isProtected = true;
        }
        else
        {
            if(inactiveSpikeTimeElapsed >= spikesReactivationTime)
            {
                isSpikesActive = true;
                bodiesAnimator[bodiesAnimator.Count - 1].SetBool("IsTailBroken", false);
            }
            inactiveSpikeTimeElapsed += Time.fixedDeltaTime;
            spikeDisplay.SetActive(false);

            if(currentSheepShield != null && currentSheepShield.isActive)
            {
                isProtected = true;
            }
            else
            {
                isProtected = false;
            }
        }
    }

    public void DisableSpikes()
    {
        isSpikesActive = false;
        inactiveSpikeTimeElapsed = 0;
        bodiesAnimator[bodiesAnimator.Count - 1].SetBool("IsTailBroken", true);
        //ajouter anim et effet
    }

    Vector2 newBodyPartPos = Vector2.zero;
    Vector2 newBodyPartDirectionFromPrevious;
    private void UpdateBodyPos()
    {
        for (int i = 0; i < bodiesRb.Count; i++)
        {
            while (Vector2.Distance(movementTrailPos[currentBodyTrailPosIndex[i]], i == 0 ? (Vector2)transform.position : movementTrailPos[currentBodyTrailPosIndex[i - 1]]) > maxDistanceBetweenBodyParts[i])
            {
                currentBodyTrailPosIndex[i]++;
            }
            bodiesRb[i].transform.position = movementTrailPos[currentBodyTrailPosIndex[i]];

            Vector2 direction = movementTrailPos[currentBodyTrailPosIndex[i]] - (Vector2)bodiesRb[i].position;
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
    }

    public override void DamageEffect()
    {
        maxSpeed = 0;
    }

    protected override void OnDie()
    {
        base.OnDie();
        for (int i = 0; i < bodiesAnimator.Count; i++)
        {
            bodiesAnimator[i].SetBool("Dead", true);
        }
    }
    protected override void OnActivate()
    {
        rb.WakeUp();
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        if (!isProtected)
        {
            TakeDamage(damage, 0.5f);
        }
        return isSpikesActive;
    }

    private void UpdateVisuals()
    {
        if (isFacingLeft && !headSprite.flipY)
        {
            headSprite.flipY = true;
            headSprite.transform.localRotation = Quaternion.Euler(0, 0, -angleCorrection);
            headSprite.transform.localPosition = new Vector3(0, -headHeightCorrection, 0);
        }
        if (!isFacingLeft && headSprite.flipY)
        {
            headSprite.flipY = false;
            headSprite.transform.localRotation = Quaternion.Euler(0, 0, angleCorrection);
            headSprite.transform.localPosition = new Vector3(0, headHeightCorrection, 0);
        }

        for (int i = 0; i < bodiesRb.Count; i++)
        {
            bodiesRb[i].transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, bodiesRb[i].velocity));

            if (bodiesRb[i].velocity.x < 0 && !bodiesSprite[i].flipY)
            {
                bodiesSprite[i].flipY = true;
                bodiesSprite[i].transform.localRotation = Quaternion.Euler(0, 0, -angleCorrection);
            }
            if (bodiesRb[i].velocity.x > 0 && bodiesSprite[i].flipY)
            {
                bodiesSprite[i].flipY = false;
                bodiesSprite[i].transform.localRotation = Quaternion.Euler(0, 0, angleCorrection);
            }
        }
    }
}
