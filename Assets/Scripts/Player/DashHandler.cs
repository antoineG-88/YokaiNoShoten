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
    public GameObject shadowFx;
    [Header("Attack settings")]
    public Vector2 attackRange;
    public int attackDamage;
    public float attackKnockbackForce;
    public float maxSlowMoTime;
    public bool hitResetDash;
    public GameObject attackFx;

    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isReaiming;
    private Vector2 dashDirection;
    private bool leftTriggerPressed;
    private bool leftTriggerDown;
    private Rigidbody2D rb;
    private ContactFilter2D enemyFilter;
    private ContactFilter2D attackReactionFilter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftTriggerPressed = false;
        leftTriggerDown = false;
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        attackReactionFilter.SetLayerMask(LayerMask.GetMask("DashInteraction"));
        attackReactionFilter.useTriggers = true;
        enemyFilter.useTriggers = true;
    }

    void Update()
    {
        UpdateInputs();
    }

    private void FixedUpdate()
    {
        if(GameData.movementHandler.isGrounded && !isDashing)
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
        isDashing = true;
        canDash = false;
        isReaiming = false;
        GameData.movementHandler.isAffectedbyGravity = false;
        bool hitAnEnemy = false;

        Vector2 dashStartPos = transform.position;
        Vector2 startDashDirection = dashDirection;
        Vector2 dashEndPos = (Vector2)transform.position + startDashDirection * dashDistance;
        Vector2 dashPos = transform.position;
        Vector2 previousDashPos = transform.position;
        float currentDashSpeed;
        GameData.grappleHandler.ReleaseHook();
        GameData.movementHandler.canMove = false;
        hitAnEnemy = Attack(startDashDirection);

        float dashTimeElapsed = 0;
        while(dashTimeElapsed < dashTime && GameData.playerManager.inControl)
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
        if(hitAnEnemy && GameData.playerManager.inControl)
        {
            StartCoroutine(SlowMoDash());
        }
    }

    private bool Attack(Vector2 attackDirection)
    {
        bool hasHit = false;
        Instantiate(attackFx, (Vector2)transform.position + attackDirection * attackRange.x * 0.5f, Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, attackDirection))));

        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapBox((Vector2)transform.position + attackDirection * attackRange.x * 0.5f, attackRange, Vector2.SignedAngle(Vector2.right, attackDirection), enemyFilter ,colliders);
        if(colliders.Count > 0)
        {
            foreach(Collider2D collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                enemy.TakeDamage(attackDamage, attackDirection * attackKnockbackForce, 0.5f);
            }
            canDash = hitResetDash ? true : canDash;
            hasHit = true;
        }
        Physics2D.OverlapBox((Vector2)transform.position + attackDirection * attackRange.x * 0.5f, attackRange, Vector2.SignedAngle(Vector2.right, attackDirection), attackReactionFilter, colliders);
        if (colliders.Count > 0)
        {
            foreach (Collider2D collider in colliders)
            {
                DashInteraction element = collider.GetComponent<DashInteraction>();
                element.DashReaction();
            }
        }
        return hasHit;
    }

    private IEnumerator SlowMoDash()
    {
        GameData.slowMoManager.StartSlowMo(1);
        isReaiming = true;

        float timeRemaining = Time.realtimeSinceStartup + maxSlowMoTime;

        while (isReaiming && timeRemaining > Time.realtimeSinceStartup)
        {
            yield return new WaitForEndOfFrame();
        }
        isReaiming = false;
        GameData.slowMoManager.StopSlowMo();
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
