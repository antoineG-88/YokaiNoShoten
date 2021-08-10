using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerVisuals : MonoBehaviour
{
    public AnimationClip dashAttackClip;
    public AnimationClip pierceAnimClip;

    [HideInInspector] public bool facingRight;

    private SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    private bool transformFacingRight;
    private bool useCustomRotation;
    private bool isDashRotated;
    private bool wasPiercing;
    private float pierceTimeElapsed;
    private Vector2 dashDirection;
    public ParticleSystem dashParticle;
    public ParticleSystem pierceParticle;
    float shapeAngle;
    float pierceShapeAngle;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        transformFacingRight = true;

        //System particle dash
        //dashParticle = new SerializedObject(GetComponent<ParticleSystem>());

        ParticleSystem.ShapeModule shapeD = dashParticle.shape;
        shapeD.rotation = new Vector3(shapeAngle, -90, 90);

        ParticleSystem.ShapeModule shapeP = pierceParticle.shape;
        shapeP.rotation = new Vector3(0, 0, pierceShapeAngle);

    }

    void Update()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (GameData.grappleHandler.isTracting)
        {
            facingRight = GameData.grappleHandler.tractionDirection.x > 0 ? true : false;
        }
        else if (GameData.dashHandler.isDashing)
        {
            facingRight = dashDirection.x > 0 ? true : false;

        }
        else
        {
            if (wasPiercing)
            {
                facingRight = GameData.pierceHandler.piercableDirection.x > 0 ? true : false;
            }
            else
            {
                if (GameData.movementHandler.isOnSlidingSlope)
                {
                    //facingRight = GameData.movementHandler.rb.velocity.x > 0 ? true : false; //A changer
                    RaycastHit2D groundHit = Physics2D.Raycast(transform.parent.position, Vector2.down, 5, LayerMask.GetMask("Wall", "DashWall", "Platform"));

                    facingRight = groundHit.normal.x > 0 ? true : false;
                }
                else
                {
                    if (GameData.movementHandler.horizontalTargetSpeed != 0)
                    {
                        facingRight = GameData.movementHandler.horizontalTargetSpeed > 0 ? true : false;
                    }
                }
            }
        }

        if (wasPiercing)
        {
            pierceTimeElapsed += Time.deltaTime;
            if (pierceTimeElapsed > pierceAnimClip.length || GameData.dashHandler.isDashing || GameData.grappleHandler.isTracting)
            {
                wasPiercing = false;
            }
        }

        if (facingRight != transformFacingRight)
        {
            transformFacingRight = facingRight;
            FlipTransform(facingRight);
        }

        useCustomRotation = GameData.grappleHandler.isTracting || isDashRotated || GameData.pierceHandler.isPiercing || wasPiercing;
        if (useCustomRotation)
        {
            if (GameData.grappleHandler.isTracting)
            {
                transform.localRotation = Quaternion.Euler(0, 0, GameData.grappleHandler.tractionDirection.x < 0 ? Vector2.SignedAngle(new Vector2(-1, 1.3f), GameData.grappleHandler.tractionDirection) : Vector2.SignedAngle(new Vector2(1, 1.3f), GameData.grappleHandler.tractionDirection));
            }
            else if (GameData.pierceHandler.isPiercing || wasPiercing)
            {
                Debug.DrawRay(transform.position, GameData.pierceHandler.piercableDirection * 3);
                transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(GameData.pierceHandler.piercableDirection.x < 0 ? Vector2.left : Vector2.right, GameData.pierceHandler.piercableDirection));
            }
            else if (isDashRotated)
            {
                transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(dashDirection.x <= 0 ? Vector2.left : Vector2.right, dashDirection));
            }
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }

        {

        }



        UpdateAnimator();
    }

    private void FlipTransform(bool facing)
    {
        if (facing)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;

        }
    }

    public void RotatePierce()
    {
        wasPiercing = true;
        pierceTimeElapsed = 0;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("TargetSpeed", Mathf.Abs(GameData.movementHandler.horizontalTargetSpeed));
        animator.SetFloat("VerticalSpeed", GameData.grappleHandler.rb.velocity.y);
        animator.SetBool("InTheAir", !GameData.movementHandler.isGrounded);
        animator.SetBool("IsTracting", GameData.grappleHandler.isTracting);
        animator.SetBool("IsSliding", GameData.movementHandler.isOnSlidingSlope);
        animator.SetBool("IsDrifting", GameData.movementHandler.currentGravityZone != null);

    }

    public IEnumerator SetDashRotation(Vector2 lastDashDirection)
    {
        isDashRotated = true;
        dashDirection = lastDashDirection;

        //System particle dash
        shapeAngle = Vector2.SignedAngle(Vector2.right, dashDirection);
        ParticleSystem.ShapeModule shapeD = dashParticle.shape;
        shapeD.rotation = new Vector3(shapeAngle, -90, 90);

        //System particle pierce
        pierceShapeAngle = Vector2.SignedAngle(Vector2.right, GameData.pierceHandler.piercableDirection);
        ParticleSystem.ShapeModule shapeP = pierceParticle.shape;
        shapeP.rotation = new Vector3(0, 0, pierceShapeAngle-90);

        yield return new WaitForSeconds(dashAttackClip.length);
        isDashRotated = false;
    }

}
