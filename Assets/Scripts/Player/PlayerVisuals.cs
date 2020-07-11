using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [HideInInspector] public bool facingRight;
    [HideInInspector] public int isHurt;

    [HideInInspector] public Animator animator;
    private bool transformFacingRight;
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
        animator.SetBool("IsRunning", Mathf.Abs(GameData.movementHandler.horizontalTargetSpeed) != 0 ? true : false);

        animator.SetFloat("VerticalSpeed", GameData.movementHandler.rb.velocity.y);

        animator.SetBool("IsTracting", GameData.grappleHandler.isTracting);

        //animator.SetBool("IsDashing", GameData.playerMovement.isDashing);

        animator.SetBool("IsInTheAir", !GameData.movementHandler.isGrounded);

        animator.SetBool("IsFacingRight", facingRight);

        /*animator.SetBool("IsKicking", isKicking > 0 ? true : false);
        if (isKicking > 0)
        {
            isKicking--;
        }

        animator.SetBool("IsCastingPower", isCastingPower > 0 ? true : false);
        if (isCastingPower > 0)
        {
            isCastingPower--;
        }

        animator.SetBool("IsSlaming", isSlaming);

        animator.SetBool("IsHurt", isHurt > 0 ? true : false);
        if (isHurt > 0)
        {
            isHurt--;
        }

        animator.SetBool("IsDying", GameData.playerManager.isDead);*/
    }
}
