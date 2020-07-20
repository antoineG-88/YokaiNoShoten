using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : Enemy
{
    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float provocationRange;
    public float safeDistanceToPlayer;
    public float safeDistanceWidth;
    [Header("RushAttack settings")]
    public float rushCooldown;
    public float rushTriggerDistance;
    public float rushTriggerTime;
    public float rushLength;
    public float rushTime;
    public float rushRadius;
    public float rushStunTime;
    public float rushKnockbackForce;
    public AnimationCurve rushCurve;
    [Header("Visuals")]
    public SpriteRenderer sprite;


    private bool fleeing;
    private bool destinationReached;
    private bool isApproaching;
    private float distToPlayer;
    private bool pathNeedUpdate;
    private bool isRushing;
    private float rushCoolDownRemaining;
    private float rushTriggerTimeElapsed;
    private Vector2 playerDirection;
    private bool isFacingRight;

    private AntiGrabShieldHandler antiGrabShieldHandler;

    new void Start()
    {
        base.Start();
        provoked = false;
        fleeing = false;
        destinationReached = false;
        isRushing = false;
        rushCoolDownRemaining = 0;
        antiGrabShieldHandler = GetComponent<AntiGrabShieldHandler>();
    }

    new void Update()
    {
        base.Update();
        UpdateBehavior();
        UpdateVisuals();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateShield();
    }

    public override void UpdateMovement()
    {
        if(!isRushing)
        {
            isFacingRight = pathDirection.x > 0;
            if (path != null && !pathEndReached && !destinationReached && inControl)
            {
                Vector2 force = new Vector2(pathDirection.x * accelerationForce, pathDirection.y * accelerationForce);

                rb.velocity += force * Time.fixedDeltaTime;

                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }
            else if (destinationReached)
            {
                rb.velocity -= rb.velocity.normalized * accelerationForce * Time.fixedDeltaTime;
                if (rb.velocity.magnitude <= accelerationForce * Time.fixedDeltaTime)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    private void UpdateBehavior()
    {
        distToPlayer = Vector2.Distance(transform.position, GameData.player.transform.position);
        pathNeedUpdate = false;
        if (provoked && inControl)
        {
            fleeing = distToPlayer < safeDistanceToPlayer && rushCoolDownRemaining > 0;

            if (rushCoolDownRemaining <= 0)
            {
                destinationReached = distToPlayer < rushTriggerDistance;

                if (distToPlayer < rushTriggerDistance)
                {
                    animator.SetTrigger("Hurt");
                    rushTriggerTimeElapsed += Time.deltaTime;

                    if (rushTriggerTimeElapsed > rushTriggerTime)
                    {
                        playerDirection = GameData.player.transform.position - transform.position;
                        playerDirection.Normalize();
                        StartCoroutine(Rush(playerDirection));
                    }
                }
                else
                {
                    rushTriggerTimeElapsed = 0;
                }
            }
            else
            {
                rushCoolDownRemaining -= Time.deltaTime;

                if (destinationReached != distToPlayer >= safeDistanceToPlayer && distToPlayer < safeDistanceToPlayer + safeDistanceWidth)
                {
                    pathNeedUpdate = true;
                }

                destinationReached = distToPlayer >= safeDistanceToPlayer && distToPlayer < safeDistanceToPlayer + safeDistanceWidth;
            }



            if(!fleeing)
            {
                targetPathfindingPosition = GameData.player.transform.position;
            }
            else
            {
                Vector2 playerOppositeDirection = transform.position - GameData.player.transform.position;
                playerOppositeDirection.Normalize();
                targetPathfindingPosition = (Vector2)transform.position + playerOppositeDirection * 3;
            }

            if(pathNeedUpdate)
            {
                UpdatePath();
            }
        }
        else
        {
            provoked = Vector2.Distance(transform.position, GameData.player.transform.position) < provocationRange;
        }
    }

    private IEnumerator Rush(Vector2 rushDirection)
    {
        isRushing = true;
        rushCoolDownRemaining = rushCooldown;
        rushTriggerTimeElapsed = 0;
        inControl = false;

        Vector2 rushStartPos = transform.position;
        Vector2 dashEndPos = (Vector2)transform.position + rushDirection * rushLength;
        Vector2 rushPos = transform.position;
        Vector2 previousRushPos = transform.position;
        float currentRushSpeed;
        bool hasHit = false;

        float dashTimeElapsed = 0;
        while (dashTimeElapsed < rushTime)
        {
            dashTimeElapsed += Time.fixedDeltaTime;
            rushPos = Vector2.LerpUnclamped(rushStartPos, dashEndPos, rushCurve.Evaluate(dashTimeElapsed / rushTime));
            currentRushSpeed = (rushPos - previousRushPos).magnitude;
            previousRushPos = rushPos;
            rb.velocity = rushDirection * currentRushSpeed * (1 / Time.fixedDeltaTime);

            if(!hasHit)
            {
                hasHit = Physics2D.OverlapCircle(transform.position, rushRadius, LayerMask.GetMask("Player"));
                if (hasHit)
                {
                    GameData.playerManager.LoseSpiritParts(1, rushDirection * rushKnockbackForce);
                }
            }

            yield return new WaitForFixedUpdate();
        }
        isRushing = false;
        yield return new WaitForSeconds(rushStunTime);
        inControl = true;
    }

    private void UpdateShield()
    {
        if(isFacingRight)
        {
            antiGrabShieldHandler.ChangeShieldAngle(antiGrabShieldHandler.shields[0], 0);
        }
        else
        {
            antiGrabShieldHandler.ChangeShieldAngle(antiGrabShieldHandler.shields[0], 180);
        }
    }

    private void UpdateVisuals()
    {
        if(isFacingRight && !sprite.flipX)
        {
            sprite.flipX = true;
        }
        if (!isFacingRight && sprite.flipX)
        {
            sprite.flipX = false;
        }
    }
}
