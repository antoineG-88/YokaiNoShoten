﻿using System.Collections;
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
    private bool wallAhead;

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
            targetPathfindingPosition = GameData.player.transform.position;
        }
        else
        {
            targetPathfindingPosition = transform.position;
        }
        if (provoked)
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
            //DropBomb();
        }
    }

    private void IsPlayerApproaching()
    {
        bool playerIsLeft = Vector2.SignedAngle(currentDirection, GameData.player.transform.position - transform.position) > 0;
        detectionZoneCenterOffset.x = Mathf.Cos(currentAngle + 90 * (playerIsLeft ? 1 : -1));
        detectionZoneCenterOffset.y = Mathf.Sin(currentAngle + 90 * (playerIsLeft ? 1 : -1));
        detectionZoneCenterOffset *= dashDetectionZone.y / 2;
        if (Physics2D.OverlapBox((Vector2)transform.position + detectionZoneCenterOffset, dashDetectionZone, currentAngle))
        {

        }
    }

    private void DropBomb()
    {
        Instantiate(bombPrefab, bombDropPos.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if(Application.isEditor)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y - dashDetectionZone.y / 2), dashDetectionZone);
        }
        else if(Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, currentAngle));
            Gizmos.DrawWireCube((Vector2)transform.position + detectionZoneCenterOffset, dashDetectionZone);
        }
    }
}