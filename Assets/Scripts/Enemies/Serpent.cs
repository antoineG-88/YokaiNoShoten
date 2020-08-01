using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpent : Enemy
{
    [Header("Movement settings")]
    public float speedToReachPlayer;
    public float speedPatrolingPlayer;
    public float accelerationForce;
    public float wallDetectionDistance;
    public float slowSpeed;
    public float provocationRange;
    public float rangeGoalToPlayer;
    public float steeringRatio;
    public float minAngleDiffToTurn;
    [Header("Bomb settings")]
    public Vector2 dashDetectionZone;
    public float dashDropCooldown;
    public float dashDistance;
    public float dashTime;
    public float dashDroppedBombNumber;
    public Transform bombDropPos;
    public GameObject bombPrefab;
    //[Header("Technical settings")]

    private float timeBeforeNextBombDrop;
    private bool isTooFarFromPlayer;
    private float currentSpeed;
    private Vector2 currentDirection;
    private float currentAngle;
    private float targetSpeed;
    private float angleDifferenceToTarget;
    private bool wallAhead;
    private bool isDashing;
    private float dashCooldownRemaining;

    private Vector2 detectionZoneCenterOffset;

    private new void Start()
    {
        base.Start();
        provoked = false;
        timeBeforeNextBombDrop = dashDropCooldown;
        currentDirection = Vector2.up;
    }

    private new void Update()
    {
        base.Update();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        IsPlayerApproaching();
    }

    public override void UpdateMovement()
    {
        if (isTooFarFromPlayer)
        {
            targetPathfindingPosition = FindNearestSightSpot(5, rangeGoalToPlayer, false);
        }
        else
        {
            targetPathfindingPosition = transform.position;
        }
        if (provoked && !isDashing)
        {
            wallAhead = Physics2D.Raycast(transform.position, currentDirection, wallDetectionDistance, LayerMask.GetMask("Wall"));
            targetSpeed = wallAhead ? slowSpeed : isTooFarFromPlayer ? speedToReachPlayer : speedPatrolingPlayer;

            if(!inControl)
            {
                currentSpeed = rb.velocity.magnitude;
                currentDirection = rb.velocity.normalized;
            }


            if (Mathf.Abs(currentSpeed - targetSpeed) <= 0.01f)
            {
                currentSpeed = targetSpeed;
            }
            else if(currentSpeed < targetSpeed)
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
                angleDifferenceToTarget = Vector2.SignedAngle(currentDirection, GetPathNextPosition(2) - (Vector2)transform.position);
                if(Mathf.Abs(angleDifferenceToTarget) > minAngleDiffToTurn)
                {
                    currentAngle += Mathf.Sign(angleDifferenceToTarget) * steeringRatio * currentSpeed * Time.fixedDeltaTime;
                    currentDirection = DirectionFromAngle(currentAngle);
                }
            }
            else if(wallAhead)
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
        else
        {
            provoked = distToPlayer < provocationRange;
        }

        if(dashCooldownRemaining > 0)
        {
            dashCooldownRemaining -= Time.fixedDeltaTime;
        }
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        isTooFarFromPlayer = distToPlayer > rangeGoalToPlayer;
        if(timeBeforeNextBombDrop > 0)
        {
            timeBeforeNextBombDrop -= Time.deltaTime;
        }
        else
        {
            timeBeforeNextBombDrop = dashDropCooldown;
        }
    }

    private void IsPlayerApproaching()
    {
        bool playerIsLeft = Vector2.SignedAngle(currentDirection, GameData.player.transform.position - transform.position) > 0;
        detectionZoneCenterOffset.x = Mathf.Cos(Mathf.Deg2Rad * (currentAngle + 90 * (playerIsLeft ? 1 : -1)));
        detectionZoneCenterOffset.y = Mathf.Sin(Mathf.Deg2Rad * (currentAngle + 90 * (playerIsLeft ? 1 : -1)));
        detectionZoneCenterOffset *= dashDetectionZone.y / 2;
        if (!isDashing && dashCooldownRemaining <= 0 && Physics2D.OverlapBox((Vector2)transform.position + detectionZoneCenterOffset, dashDetectionZone, currentAngle, LayerMask.GetMask("Player")))
        {
            dashCooldownRemaining = dashDropCooldown;
            StartCoroutine(DashDrop());
        }
    }

    private IEnumerator DashDrop()
    {
        isDashing = true;
        Vector2 direction = currentDirection;
        float timer = 0;
        float timeBewteenBombDrop = dashTime / dashDroppedBombNumber;
        float timeSinceLastDrop = 0;
        float speed = dashDistance / dashTime;
        while(timer < dashTime && inControl)
        {
            rb.velocity = direction * speed;
            if(timeSinceLastDrop > timeBewteenBombDrop)
            {
                Instantiate(bombPrefab, bombDropPos.position, Quaternion.identity);
                timeSinceLastDrop = 0;
            }
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            timeSinceLastDrop += Time.fixedDeltaTime;
        }
        isDashing = false;
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if(!Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y - dashDetectionZone.y / 2), dashDetectionZone);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.matrix = Matrix4x4.TRS((Vector2)transform.position + detectionZoneCenterOffset, Quaternion.Euler(0, 0, currentAngle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, dashDetectionZone);
        }
    }
}
