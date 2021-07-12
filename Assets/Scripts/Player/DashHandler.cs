using System.Collections;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
    [Header("Dash settings")]
    public float dashDistance;
    public float dashTime;
    public AnimationCurve dashCurve;
    public float dashEndVelocityForceAdded;
    public bool useVelocity;
    public GameObject shadowFx;
    public bool dashWithRightTrigger;
    public bool aimWithRightJoystick;

    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isReaiming;
    [HideInInspector] public Vector2 dashDirection;
    private bool dashTriggerPressed;
    private bool dashTriggerDown;
    private Rigidbody2D rb;
    private ContactFilter2D enemyFilter;
    private ContactFilter2D attackReactionFilter;
    private Vector2 defaultDashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashTriggerPressed = false;
        dashTriggerDown = false;
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        attackReactionFilter.SetLayerMask(LayerMask.GetMask("DashInteraction"));
        attackReactionFilter.useTriggers = true;
        enemyFilter.useTriggers = true;
        defaultDashDirection = Vector2.up;
    }

    void Update()
    {
        UpdateInputs();
    }

    private void FixedUpdate()
    {
        if(GameData.movementHandler.isGrounded && !isDashing && !GameData.movementHandler.isOnSlope)
        {
            canDash = true;
        }
    }

    private void UpdateInputs()
    {
        DashTriggerUpdate();

        if (GameData.grappleHandler.isTracting)
        {
            defaultDashDirection = GameData.grappleHandler.tractionDirection;
        }
        else
        {
            defaultDashDirection = GameData.grappleHandler.aimDirection;
        }

        dashDirection = aimWithRightJoystick ? new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV")) : new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV"));

        if (dashDirection.magnitude <= 0.1f)
        {
            dashDirection = defaultDashDirection;
        }

        dashDirection.Normalize();

        if (dashTriggerDown && !isDashing && canDash)
        {
            StartCoroutine(Dash(dashDirection));
        }

    }

    private IEnumerator Dash(Vector2 startDashDirection)
    {
        isDashing = true;
        canDash = false;
        //isReaiming = false;
        GameData.pierceHandler.StopPhasingTime();
        GameData.movementHandler.isAffectedbyGravity = false;
        GameData.playerVisuals.animator.SetTrigger("DashAttack");
        StartCoroutine(GameData.playerVisuals.SetDashRotation(dashDirection));

        Vector2 dashStartPos = transform.position;

        Vector2 dashEndPos = (Vector2)transform.position + startDashDirection * dashDistance;
        Vector2 dashPos = transform.position;
        Vector2 previousDashPos = transform.position;
        float currentDashSpeed;
        GameData.grappleHandler.ReleaseHook();
        GameData.movementHandler.canMove = false;

        float dashTimeElapsed = 0;
        while(dashTimeElapsed < dashTime && GameData.playerManager.inControl && isDashing)
        {
            dashTimeElapsed += Time.fixedDeltaTime;
            Instantiate(shadowFx, transform.position, Quaternion.identity).transform.localScale = new Vector3(startDashDirection.x > 0 ? 1 : -1, 1, 1);
            dashPos = Vector2.LerpUnclamped(dashStartPos, dashEndPos, dashCurve.Evaluate(dashTimeElapsed / dashTime));
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
        //transform.position = dashEndPos;  Test de raycast à faire si utilisé

        rb.velocity += rb.velocity.normalized * dashEndVelocityForceAdded;

        GameData.movementHandler.canMove = true;
        GameData.movementHandler.isAffectedbyGravity = true;
        isDashing = false;
    }

    private void DashTriggerUpdate()
    {
        if (!dashTriggerPressed && (dashWithRightTrigger ? Input.GetAxisRaw("RightTrigger") == 1 : Input.GetAxisRaw("LeftTrigger") == 1))
        {
            dashTriggerDown = true;
        }
        else
        {
            dashTriggerDown = false;
        }

        if (dashWithRightTrigger ? Input.GetAxisRaw("RightTrigger") == 1 : Input.GetAxisRaw("LeftTrigger") == 1)
        {
            dashTriggerPressed = true;
        }
        else
        {
            dashTriggerPressed = false;
            dashTriggerDown = false;
        }
    }
}
