using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [HideInInspector] public bool facingRight;

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
        animator.SetFloat("TargetSpeed", Mathf.Abs(GameData.movementHandler.horizontalTargetSpeed));
        animator.SetFloat("VerticalSpeed", GameData.grappleHandler.rb.velocity.y);
        animator.SetBool("InTheAir", !GameData.movementHandler.isGrounded);
        animator.SetBool("IsTracting", GameData.grappleHandler.isTracting);
    }
}
