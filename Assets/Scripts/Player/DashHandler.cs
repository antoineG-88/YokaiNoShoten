using System.Collections;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
    [Header("Dash settings")]
    public float dashDistance;
    public float dashTime;
    public AnimationCurve dashCurveSetDashRotation;
    public float dashEndVelocityForceAdded;
    public GameObject shadowFx;
    public bool dashWithRightTrigger;
    public bool aimWithRightJoystick;
    public Sound dashSound;

    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isReaiming;
    [HideInInspector] public Vector2 dashDirection;
    [HideInInspector] public bool dashTriggerPressed;
    [HideInInspector] public bool dashTriggerDown;
    private Rigidbody2D rb;
    private ContactFilter2D enemyFilter;
    private ContactFilter2D attackReactionFilter;
    private Vector2 defaultDashDirection;
    public AnimationCurve dashCurve;
    

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
        if(GameData.movementHandler.isGrounded && !isDashing && !GameData.movementHandler.isOnSlidingSlope)
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

        if(GameManager.isUsingController)
        {
            dashDirection = aimWithRightJoystick ? new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV")) : new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV"));
        }
        else
        {
            dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        if (dashDirection.magnitude <= 0.1f)
        {
            dashDirection = defaultDashDirection;
        }

        dashDirection.Normalize();

        if (dashTriggerDown && !isDashing && canDash && ((GameData.pierceHandler.selectedEnemy == null && GameData.pierceHandler.useLeftTriggerInput) || !GameData.pierceHandler.useLeftTriggerInput) && GameData.playerManager.inControl)
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
        //GameData.movementHandler.isAffectedbyGravity = false;
        GameData.playerVisuals.animator.SetTrigger("DashAttack");
        GameData.playerVisuals.dashParticle.Play();
        StartCoroutine(GameData.playerVisuals.SetDashRotation(dashDirection));
        if(dashSound != null)
            GameData.playerSource.PlayOneShot(dashSound.clip, dashSound.volumeScale);

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
            GameObject shadowClone = Instantiate(shadowFx, transform.position, Quaternion.identity);
            shadowClone.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, startDashDirection));
            shadowClone.transform.localScale = new Vector3(1, dashDirection.x > 0 ? 1 : -1, 1);
            dashPos = Vector2.LerpUnclamped(dashStartPos, dashEndPos, dashCurve.Evaluate(dashTimeElapsed / dashTime));
            currentDashSpeed = (dashPos - previousDashPos).magnitude;
            previousDashPos = dashPos;

            rb.velocity = startDashDirection * currentDashSpeed * (1 / Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        if(!GameData.playerManager.inControl || !isDashing)
        {
            dashPos = dashEndPos;
            previousDashPos = Vector2.LerpUnclamped(dashStartPos, dashEndPos, 1 - Time.fixedDeltaTime);
            currentDashSpeed = (dashPos - previousDashPos).magnitude;
            rb.velocity = startDashDirection * currentDashSpeed * (1 / Time.fixedDeltaTime);
        }

        rb.velocity += rb.velocity.normalized * dashEndVelocityForceAdded;
        GameData.playerVisuals.dashParticle.Stop();

        GameData.movementHandler.canMove = true;
        isDashing = false;
    }

    private void DashTriggerUpdate()
    {
        if (GameManager.isUsingController)
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
        else
        {
            dashTriggerDown = Input.GetButtonDown("Dash");
            dashTriggerPressed = Input.GetButton("Dash");
        }
    }
}
