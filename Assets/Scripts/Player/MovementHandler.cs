﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public float walkingMaxSpeed;
    public float walkingMinSpeed;
    public float walkingAcceleration;
    public float airAcceleration;
    public float groundSlowing;
    public float airSlowing;
    public float gravityForce;
    [Space]
    [Header("References")]
    public Collider2D feetCollider;

    [HideInInspector] public float horizontalTargetSpeed;
    private float currentAcceleration;
    private float currentSlowing;
    private float horizontalForce;
    private float forceSign;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool isAffectedbyGravity;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Rigidbody2D groundRb;
    private ContactFilter2D groundFilter;
    private Vector2 relativeVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrounded = false;
        groundFilter.SetLayerMask(LayerMask.GetMask("Wall"));
        groundFilter.useTriggers = true;
        canMove = true;
        isAffectedbyGravity = true;
    }

    void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        isGrounded = IsOnGround();
        UpdateMovement();
    }

    private void GetInputs()
    {
        horizontalTargetSpeed = canMove && GameData.playerManager.inControl ? Input.GetAxis("LeftStickH") * walkingMaxSpeed : 0;
        if (Mathf.Abs(horizontalTargetSpeed) <= walkingMinSpeed)
        {
            horizontalTargetSpeed = 0;
        }
    }

    private void UpdateMovement()
    {
        if(groundRb != null)
        {
            relativeVelocity = rb.velocity - groundRb.velocity;
        }
        else
        {
            relativeVelocity = rb.velocity;
        }

        if (horizontalTargetSpeed != relativeVelocity.x)
        {
            currentAcceleration = isGrounded ? walkingAcceleration : airAcceleration;
            currentSlowing = isGrounded ? groundSlowing : airSlowing;

            forceSign = Mathf.Sign(horizontalTargetSpeed - relativeVelocity.x);
            if (horizontalTargetSpeed > 0 && relativeVelocity.x < horizontalTargetSpeed || horizontalTargetSpeed < 0 && relativeVelocity.x > horizontalTargetSpeed && canMove && GameData.playerManager.inControl)
            {
                horizontalForce = forceSign * currentAcceleration * Time.fixedDeltaTime;
            }
            else
            {
                horizontalForce = forceSign * currentSlowing * Time.fixedDeltaTime;
            }


            if (horizontalTargetSpeed > relativeVelocity.x && horizontalTargetSpeed < relativeVelocity.x + horizontalForce || horizontalTargetSpeed < relativeVelocity.x && horizontalTargetSpeed > relativeVelocity.x + horizontalForce)
            {
                rb.velocity = new Vector2(groundRb != null ? groundRb.velocity.x : 0 + horizontalTargetSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x + horizontalForce, rb.velocity.y);
            }
        }
        else if(groundRb != null)
        {
            rb.velocity = new Vector2(groundRb.velocity.x, groundRb.velocity.y);
        }

        if (isAffectedbyGravity)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityForce * Time.fixedDeltaTime);
        }
    }

    private bool IsOnGround()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(feetCollider, groundFilter, colliders);
        if(colliders.Count > 0)
        {
            return true;
        }
        return false;
    }

    public void Propel(Vector2 directedForce, bool resetMomentum)
    {
        if(resetMomentum)
        {
            rb.velocity = directedForce;
        }
        else
        {
            rb.velocity += directedForce;
        }
    }
}
