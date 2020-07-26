using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpent : Enemy
{
    [Header("Movement settings")]
    public float speedToReachPlayer;
    public float speedPatrolingPlayer;
    public float accelerationForce;
    public float slowingWallDistance;
    public float slowSpeed;
    public float provocationRange;
    public float rangeGoalToPlayer;
    public float steeringRatio;
    public float minAngleDiffToTurn;
    [Header("Bomb settings")]
    public float timeBetweenBombDrop;
    public Transform bombDropPos;
    public GameObject bombPrefab;
    [Header("Technical settings")]
    public List<GameObject> bodyParts;

    private float timeBeforeNextBombDrop;
    private bool isTooFarFromPlayer;
    private float currentSpeed;
    private Vector2 currentDirection;
    private float currentAngle;
    private float targetSpeed;
    private float angleDifferenceToTarget;

    private new void Start()
    {
        base.Start();
        provoked = false;
        timeBeforeNextBombDrop = timeBetweenBombDrop;
        currentDirection = Vector2.left;
    }

    private new void Update()
    {
        base.Update();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if (isTooFarFromPlayer)
        {
            targetPathfindingPosition = GameData.player.transform.position;
        }
        else
        {
            targetPathfindingPosition = transform.position;
        }
        if (provoked)
        {
            targetSpeed = Physics2D.Raycast(transform.position, currentDirection, slowingWallDistance, LayerMask.GetMask("Wall")) ? slowSpeed : isTooFarFromPlayer ? speedToReachPlayer : speedPatrolingPlayer;

            currentSpeed = rb.velocity.magnitude;
            if (rb.velocity.magnitude != 0)
            {
                currentDirection = rb.velocity.normalized;
            }
            else
            {
                currentDirection = DirectionFromAngle(transform.rotation.eulerAngles.z);
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

            if(targetPathfindingPosition != (Vector2)transform.position)
            {
                angleDifferenceToTarget = Vector2.SignedAngle(currentDirection, GetPathNextPosition(2) - (Vector2)transform.position);
                if(Mathf.Abs(angleDifferenceToTarget) > minAngleDiffToTurn)
                {
                    currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection);
                    currentAngle += Mathf.Sign(angleDifferenceToTarget) * steeringRatio * currentSpeed * Time.fixedDeltaTime;
                    currentDirection = DirectionFromAngle(currentAngle);
                }
            }

            rb.velocity = currentDirection * currentSpeed;
            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
        }
        else
        {
            provoked = distToPlayer < provocationRange;
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
            timeBeforeNextBombDrop = timeBetweenBombDrop;
            DropBomb();
        }
    }

    private void DropBomb()
    {
        Instantiate(bombPrefab, bombDropPos.position, Quaternion.identity);
    }
}
