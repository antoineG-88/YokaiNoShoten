using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public float walkingMaxSpeed;
    public float walkingMinSpeed;
    public float walkingAcceleration;
    public float airAcceleration;
    public float slideAcceleration;
    public float groundSlowing;
    public float airSlowing;
    public float slideSlowing;
    public float gravityForce;
    public float maxSlidingSpeed;
    public AnimationCurve knockAwayMovement;
    public float knockAwayTime;
    public float slopeMaxSpeed;
    [Header("NoGravityZone settings")]
    public float NGMomentumSlowingForce;
    public float maxSpeedInNGZone;
    [Tooltip("Cursed")]
    public bool moveWithRightJoystick;
    [Space]
    [Header("References")]
    public Collider2D feetCollider;

    [HideInInspector] public float horizontalTargetSpeed;
    private float currentAcceleration;
    private float currentSlowing;
    private float horizontalForce;
    private float forceSign;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isOnSlidingSlope;
    [HideInInspector] public bool canMove;
    [HideInInspector] private bool isAffectedbyGravity;
    [HideInInspector] public bool isInNoGravityZone;
    [HideInInspector] public bool isKnockedAway;
    [HideInInspector] public int isInSlidingZone;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Rigidbody2D groundRb;
    private ContactFilter2D groundFilter;
    private Vector2 relativeVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrounded = false;
        groundFilter.SetLayerMask(LayerMask.GetMask("Wall","DashWall","Platform"));
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
        if(isGrounded)
        {
            isOnSlidingSlope = IsOnSlope() && isInSlidingZone > 0;
        }
        else
        {
            isOnSlidingSlope = false;
        }
        UpdateMovement();
    }

    private void GetInputs()
    {
        horizontalTargetSpeed = canMove && GameData.playerManager.inControl ? (moveWithRightJoystick ? Input.GetAxis("RightStickH") : Input.GetAxis("LeftStickH")) * (IsOnSlope() ? slopeMaxSpeed : walkingMaxSpeed) : 0;
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
            currentAcceleration = isInNoGravityZone ? 0 : (isGrounded ? (isOnSlidingSlope ? slideAcceleration : walkingAcceleration) : airAcceleration);
            currentSlowing = isGrounded ? isOnSlidingSlope ? slideSlowing : groundSlowing : airSlowing;

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
                rb.velocity = new Vector2((groundRb != null ? groundRb.velocity.x : 0) + horizontalTargetSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x + horizontalForce, rb.velocity.y);
            }
        }
        else if(groundRb != null)
        {
            rb.velocity = new Vector2(groundRb.velocity.x + horizontalTargetSpeed, rb.velocity.y);
        }

        if (isGrounded && isOnSlidingSlope && !GameData.dashHandler.isDashing && rb.velocity.magnitude > maxSlidingSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSlidingSpeed;
        }

        isAffectedbyGravity = !GameData.pierceHandler.isPiercing && !GameData.dashHandler.isDashing && !GameData.grappleHandler.isTracting && !isInNoGravityZone;

        if (isAffectedbyGravity)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityForce * Time.fixedDeltaTime);
        }

        if(isInNoGravityZone)
        {
            if(rb.velocity.magnitude > NGMomentumSlowingForce * Time.fixedDeltaTime)
            {
                rb.velocity -= rb.velocity * (NGMomentumSlowingForce * Time.fixedDeltaTime);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            if(!GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing && !GameData.grappleHandler.isTracting && !isKnockedAway)
            {
                if(rb.velocity.magnitude > maxSpeedInNGZone)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeedInNGZone;
                }
            }
        }
    }

    private bool IsOnGround()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(feetCollider, groundFilter, colliders);
        if(colliders.Count > 0 && !isInNoGravityZone)
        {
            return true;
        }
        return false;
    }

    private bool IsOnSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(feetCollider.transform.position, Vector2.down, 1.0f, LayerMask.GetMask("Wall", "DashWall", "Platform"));
        if (hit && Vector2.Angle(Vector2.up, hit.normal) > 30)
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

    public IEnumerator KnockAway(Vector2 directedForce)
    {
        isKnockedAway = true;

        Vector2 knockStartPos = transform.position;

        Vector2 knockEndPos = (Vector2)transform.position + directedForce;
        Vector2 currentKnockPos = transform.position;
        Vector2 previousKnockPos = transform.position;
        float currentKnockSpeed;
        canMove = false;

        float dashTimeElapsed = 0;
        while (dashTimeElapsed < knockAwayTime && isKnockedAway && !GameData.grappleHandler.isTracting && !GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing)
        {
            dashTimeElapsed += Time.fixedDeltaTime;
            currentKnockPos = Vector2.LerpUnclamped(knockStartPos, knockEndPos, knockAwayMovement.Evaluate(dashTimeElapsed / knockAwayTime));
            currentKnockSpeed = (currentKnockPos - previousKnockPos).magnitude;
            previousKnockPos = currentKnockPos;

            rb.velocity = directedForce.normalized * currentKnockSpeed * (1 / Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        canMove = true;
        isKnockedAway = false;
    }
}
