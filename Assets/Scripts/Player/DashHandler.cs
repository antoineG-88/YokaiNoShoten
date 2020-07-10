using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
    [Header("Dash settings")]
    public float dashDistance;
    public float dashTime;
    public AnimationCurve dashCurve;
    public float dashEndVelocityForceAdded;
    public bool useVelocity;
    [Header("Attack settings")]
    public Vector2 attackRange;

    [HideInInspector] public bool canDash;
    private bool isDashing;
    private Vector2 dashDirection;
    private bool leftTriggerPressed;
    private bool leftTriggerDown;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftTriggerPressed = false;
        leftTriggerDown = false;
    }

    void Update()
    {
        UpdateInputs();
    }

    private void FixedUpdate()
    {
        if(GameData.movementHandler.isGrounded)
        {
            canDash = true;
        }
    }

    private void UpdateInputs()
    {
        LeftTriggerUpdate();
        dashDirection = new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV"));
        dashDirection.Normalize();

        if (leftTriggerDown && !isDashing && canDash && dashDirection.magnitude != 0)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        GameData.movementHandler.inControl = false;
        isDashing = true;
        canDash = false;
        GameData.movementHandler.isAffectedbyGravity = false;

        Vector2 dashStartPos = transform.position;
        Vector2 startDashDirection = dashDirection;
        Vector2 dashEndPos = (Vector2)transform.position + startDashDirection * dashDistance;
        Vector2 dashPos = transform.position;
        Vector2 previousDashPos = transform.position;
        float currentDashSpeed;

        float dashTimeElapsed = 0;
        while(dashTimeElapsed < dashTime)
        {
            dashTimeElapsed += Time.fixedDeltaTime;

            dashPos = Vector2.Lerp(dashStartPos, dashEndPos, dashCurve.Evaluate(dashTimeElapsed / dashTime));
            currentDashSpeed = (dashPos - previousDashPos).magnitude;
            previousDashPos = dashPos;

            if(useVelocity)
            {
                rb.velocity = startDashDirection * currentDashSpeed * (1 / Time.fixedDeltaTime);
            }
            else
            {
                transform.position = dashPos;
            }

            yield return new WaitForFixedUpdate();
        }
        //transform.position = dashEndPos;

        //rb.velocity += rb.velocity.normalized * dashEndVelocityForceAdded;

        GameData.movementHandler.inControl = true;
        GameData.movementHandler.isAffectedbyGravity = true;
        isDashing = false;
    }

    private void LeftTriggerUpdate()
    {
        if (!leftTriggerPressed && Input.GetAxisRaw("LeftTrigger") == 1)
        {
            leftTriggerDown = true;
        }
        else
        {
            leftTriggerDown = false;
        }

        if (Input.GetAxisRaw("LeftTrigger") == 1)
        {
            leftTriggerPressed = true;
        }
        else
        {
            leftTriggerPressed = false;
            leftTriggerDown = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + (Vector3)(Vector2.right * (attackRange.x / 2)), attackRange);
    }
}
