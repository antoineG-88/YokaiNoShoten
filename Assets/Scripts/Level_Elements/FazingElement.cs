using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FazingElement : MonoBehaviour
{
    [Header("Pair = disparition, Impair = apparition")]
    public float[] fazeTimings;
    public float timeWarning;
    public float animSpeed = 1;

    private Collider2D ringCollider;
    private SpriteRenderer spriteRenderer;
    private float timeRemainingForCurrentStep;
    private int stepIndex;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ringCollider = GetComponent<Collider2D>();
        stepIndex = -1;
        animator.enabled = true;
        timeRemainingForCurrentStep = 0;
        animator.speed = animSpeed;
    }

    void Update()
    {
        if (timeRemainingForCurrentStep <= 0)
        {
            stepIndex++;
            if (stepIndex >= fazeTimings.Length)
            {
                stepIndex = 0;
            }

            if(fazeTimings[stepIndex] == 0)
            {
                stepIndex++;
                if (stepIndex >= fazeTimings.Length)
                {
                    stepIndex = 0;
                }
            }
            timeRemainingForCurrentStep = fazeTimings[stepIndex];

            if (stepIndex % 2 == 0)
            {
                FazeOut();
            }
            else
            {
                FazeIn();
            }
        }
        else
        {
            timeRemainingForCurrentStep -= Time.deltaTime;
            if(timeRemainingForCurrentStep <= timeWarning && stepIndex % 2 != 0)
            {
                animator.SetBool("Warn", true);
            }
        }
    }

    private void FazeOut()
    {
        //spriteRenderer.enabled =false;
        ringCollider.enabled = false;
        animator.SetBool("Faze", true);
        if (GameData.grappleHandler.attachedObject == gameObject)
        {
            GameData.movementHandler.rb.velocity *= GameData.grappleHandler.velocityKeptReleasingHook / 100;
            GameData.grappleHandler.BreakRope("Fazing ring");
        }
    }

    private void FazeIn()
    {
        //spriteRenderer.enabled = true;
        ringCollider.enabled = true;
        animator.SetBool("Warn", false);
        animator.SetBool("Faze", false);
    }
}
