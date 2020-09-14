using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public AnimationClip dashAttackClip;

    [HideInInspector] public bool facingRight;

    [HideInInspector] public Animator animator;
    private bool transformFacingRight;
    private bool useCustomRotation;
    private bool isDashRotated;
    void Start()
    {
        animator = GetComponent<Animator>();
        transformFacingRight = true;
    }

    void Update()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        facingRight = GameData.grappleHandler.isTracting ? (GameData.grappleHandler.tractionDirection.x > 0 ? true : false) :
                (GameData.movementHandler.horizontalTargetSpeed != 0 ? (GameData.movementHandler.horizontalTargetSpeed > 0 ? true : false) : facingRight);

        if (facingRight != transformFacingRight)
        {
            transformFacingRight = facingRight;
            FlipTransform(facingRight);
        }

        useCustomRotation = GameData.grappleHandler.isTracting || isDashRotated;
        Debug.Log(isDashRotated);
        if (useCustomRotation)
        {
            if (GameData.grappleHandler.isTracting)
            {
                transform.localRotation = Quaternion.Euler(0, 0, GameData.grappleHandler.tractionDirection.x < 0 ? Vector2.SignedAngle(new Vector2(-1, 1.3f), GameData.grappleHandler.tractionDirection) : Vector2.SignedAngle(new Vector2(1, 1.3f), GameData.grappleHandler.tractionDirection));
            }
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }

        UpdateAnimator();
    }

    private void FlipTransform(bool facing)
    {
        if (facing)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("TargetSpeed", Mathf.Abs(GameData.movementHandler.horizontalTargetSpeed));
        animator.SetFloat("VerticalSpeed", GameData.grappleHandler.rb.velocity.y);
        animator.SetBool("InTheAir", !GameData.movementHandler.isGrounded);
        animator.SetBool("IsTracting", GameData.grappleHandler.isTracting);
    }

    public IEnumerator SetDashRotation(Vector2 dashDirection)
    {
        isDashRotated = true;
        transform.localRotation = Quaternion.Euler(0, 0, dashDirection.x < 0 ? Vector2.SignedAngle(new Vector2(-1, 0), dashDirection) : Vector2.SignedAngle(new Vector2(1, 0), dashDirection));
        yield return new WaitForSeconds(dashAttackClip.length);
        isDashRotated = false;
    }
}
