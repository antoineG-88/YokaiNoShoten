using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Movement settings")]
    public float walkingMaxSpeed;
    public float walkingMinSpeed;
    public float walkingAcceleration;
    public float airAcceleration;
    public float slideAcceleration;
    public float groundSlowing;
    public float airSlowing;
    public float maxFallingSpeed;
    public float slideSlowing;
    public float gravityForce;
    public float maxSlidingSpeed;
    public AnimationCurve knockAwayMovement;
    public float knockAwayTime;
    public float slopeMaxSpeed;
    public float godModeSpeed;
    [Header("Sounds")]
    public Sound footstepSound;
    public float baseTimeBtwFootStep;
    public AudioSource windSource;
    public float windVolumeBySpeed;
    public float windVolumePower;
    public AudioSource slideSource;
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
    [HideInInspector] public int levitateSourceNumber;
    [HideInInspector] public NoGravityZone currentGravityZone;
    [HideInInspector] public bool isKnockedAway;
    [HideInInspector] public int isInSlidingZone;
    [HideInInspector] public float noControlTargetSpeed;

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
        UpdateSounds();
    }

    private void FixedUpdate()
    {
        isGrounded = IsOnGround();
        if(isGrounded)
        {
            if(IsOnSlope() && isInSlidingZone > 0 && !isOnSlidingSlope)
            {
                isOnSlidingSlope = true;
            }

            if(!IsOnSlope() && isOnSlidingSlope)
            {
                isOnSlidingSlope = false;
            }
        }
        else
        {
            isOnSlidingSlope = false;
        }
        UpdateMovement();
    }

    private void GetInputs()
    {
        if(GameData.playerManager.inControl)
        {
            horizontalTargetSpeed = canMove && GameData.playerManager.inControl ? (moveWithRightJoystick ? Input.GetAxis("RightStickH") : Input.GetAxis("LeftStickH")) * (IsOnSlope() ? slopeMaxSpeed : walkingMaxSpeed) : 0;
            if (Mathf.Abs(horizontalTargetSpeed) <= walkingMinSpeed)
            {
                horizontalTargetSpeed = 0;
            }
        }
        else
        {
            horizontalTargetSpeed = noControlTargetSpeed;
        }
    }

    private void UpdateMovement()
    {
        if(GameData.playerManager.isInGodMode)
        {
            Vector2 inputAxis = new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV"));
            inputAxis = Vector2.ClampMagnitude(inputAxis, 1);
            transform.position = (Vector2)transform.position + inputAxis * Time.fixedDeltaTime * godModeSpeed;
            rb.velocity = Vector2.zero;
        }
        else
        {
            if(GameData.playerManager.isDying)
            {

            }
            else
            {
                if (groundRb != null)
                {
                    relativeVelocity = rb.velocity - groundRb.velocity;
                }
                else
                {
                    relativeVelocity = rb.velocity;
                }

                if (horizontalTargetSpeed != relativeVelocity.x)
                {
                    currentAcceleration = (currentGravityZone != null || levitateSourceNumber > 0) ? 0 : (isGrounded ? (isOnSlidingSlope ? slideAcceleration : walkingAcceleration) : airAcceleration);
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
                else if (groundRb != null)
                {
                    rb.velocity = new Vector2(groundRb.velocity.x + horizontalTargetSpeed, rb.velocity.y);
                }

                if (isGrounded && isOnSlidingSlope && !GameData.dashHandler.isDashing && rb.velocity.magnitude > maxSlidingSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSlidingSpeed;
                }

                isAffectedbyGravity = !GameData.pierceHandler.isPiercing
                    && !GameData.dashHandler.isDashing
                    && !GameData.grappleHandler.isTracting
                    && currentGravityZone == null
                    && levitateSourceNumber <= 0
                    && !(IsOnSlope() && isInSlidingZone == 0 && horizontalTargetSpeed == 0 && isGrounded);

                levitateSourceNumber = Mathf.Clamp(levitateSourceNumber, 0, 100);
                if (isAffectedbyGravity)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityForce * Time.fixedDeltaTime);
                }

                if (currentGravityZone != null)
                {
                    if (!GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing && !GameData.grappleHandler.isTracting && !isKnockedAway)
                    {
                        if (rb.velocity.magnitude > currentGravityZone.maxSpeedInNGZone + (currentGravityZone.aboveMaxMomentumSlowingForce * Time.fixedDeltaTime))
                        {
                            rb.velocity -= rb.velocity.normalized * currentGravityZone.aboveMaxMomentumSlowingForce * Time.fixedDeltaTime;
                        }
                        else if (rb.velocity.magnitude > currentGravityZone.maxSpeedInNGZone)
                        {
                            rb.velocity = rb.velocity.normalized * currentGravityZone.maxSpeedInNGZone;
                        }
                        else if (rb.velocity.magnitude > currentGravityZone.minSpeedInNGZone + (currentGravityZone.momentumSlowingForce * Time.fixedDeltaTime))
                        {
                            rb.velocity -= rb.velocity.normalized * currentGravityZone.momentumSlowingForce * Time.fixedDeltaTime;
                        }
                        else if (rb.velocity.magnitude > currentGravityZone.minSpeedInNGZone)
                        {
                            rb.velocity = rb.velocity.normalized * currentGravityZone.minSpeedInNGZone;
                        }
                    }
                }

                if (!isGrounded)
                {
                    if (rb.velocity.y < -maxFallingSpeed && GameData.playerManager.inControl && !GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing && !GameData.playerManager.isBeingKnocked && !GameData.grappleHandler.isTracting)
                    {
                        rb.velocity = rb.velocity * Mathf.Abs(maxFallingSpeed / -rb.velocity.y);
                    }
                }
            }
        }
    }

    private bool IsOnGround()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(feetCollider, groundFilter, colliders);
        if(colliders.Count > 0 && currentGravityZone == null)
        {
            return true;
        }
        return false;
    }


    private float timeSinceLastStep;
    private void UpdateSounds()
    {
        if(isGrounded && horizontalTargetSpeed != 0 && !isOnSlidingSlope)
        {
            if(timeSinceLastStep > baseTimeBtwFootStep/* / Mathf.Abs(horizontalTargetSpeed)*/)
            {
                timeSinceLastStep = 0;
                GameData.playerSource.PlayOneShot(footstepSound.clip, footstepSound.volumeScale);
            }
            else
            {
                timeSinceLastStep += Time.deltaTime;
            }
        }
        else
        {
            timeSinceLastStep = 10;
        }


        windSource.volume = isGrounded ? 0 : Mathf.Pow(rb.velocity.magnitude * windVolumeBySpeed, windVolumePower);
        
        if(slideSource.isPlaying && !isOnSlidingSlope)
        {
            slideSource.Stop();
        }
        else if(!slideSource.isPlaying && isOnSlidingSlope)
        {
            slideSource.Play();
        }
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

        if(!GameData.playerManager.isDying)
        {
            canMove = true;
            isKnockedAway = false;
        }
        else
        {
            /*while(true)
            {
                rb.velocity = Vector2.zero;
                yield return new WaitForFixedUpdate();
            }*/
        }
    }
}
