using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public float walkingMaxSpeed;
    public float walkingMinSpeed;
    public float walkingAcceleration;
    public float airAcceleration;
    public float jumpForce;
    [Space]
    [Header("References")]
    public Collider2D feetCollider;

    private float horizontalTargetSpeed;
    private float horizontalCurrentSpeed;
    private float currentAcceleration;
    [HideInInspector] bool isGrounded;

    private Rigidbody2D rb;
    private ContactFilter2D groundFilter;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        horizontalCurrentSpeed = 0;
        isGrounded = false;
        groundFilter.SetLayerMask(LayerMask.GetMask("Wall"));
        groundFilter.useTriggers = true;
    }

    void Update()
    {
        UpdateMovementSpeed();
        if(Input.GetButton("AButton") && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Move();
        isGrounded = IsOnGround();
    }

    private void UpdateMovementSpeed()
    {
        currentAcceleration = isGrounded ? walkingAcceleration : airAcceleration;

        if (Mathf.Abs(Input.GetAxis("LeftStickH") * walkingMaxSpeed) > walkingMinSpeed)
        {
            horizontalTargetSpeed = Input.GetAxis("LeftStickH") * walkingMaxSpeed;

            horizontalCurrentSpeed += Mathf.Sign(horizontalTargetSpeed - horizontalCurrentSpeed) * currentAcceleration * Time.deltaTime;

            Mathf.Clamp(horizontalCurrentSpeed, -horizontalTargetSpeed, horizontalTargetSpeed);
        }
        else
        {
            if(Mathf.Abs(horizontalCurrentSpeed) > currentAcceleration * Time.deltaTime)
            {
                horizontalCurrentSpeed -= Mathf.Sign(horizontalCurrentSpeed) * currentAcceleration * Time.deltaTime;
            }
            else
            {
                horizontalCurrentSpeed = 0;
            }
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalCurrentSpeed, rb.velocity.y);
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

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
