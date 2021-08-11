using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : Enemy
{
    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float safeDistanceToPlayer;
    public float safeDistanceWidth;
    [Header("RushAttack settings")]
    public float rushCooldown;
    public float rushTriggerDistance;
    public float rushTriggerTime;
    public float rushLength;
    public float rushTime;
    public float rushRadius;
    public float rushWallRadius;
    public float rushStunTime;
    public float rushKnockbackForce;
    public float wallStunTime;
    public float rushDelay;
    public AnimationCurve rushCurve;
    private bool isStuckInWall = false;
    [Header("Visuals")]
    public SpriteRenderer sprite;
    public GameObject stuckImpactParticle;

    //Fx de prévisualitation
    public GameObject previsFX;
    public int numberOfPrevisFx;

    private float rangeFromInitialPos;
    private bool fleeing;
    private bool destinationReached;
    private bool isApproaching;
    private bool pathNeedUpdate;
    private bool isRushing;
    private float rushCoolDownRemaining;
    private float rushTriggerTimeElapsed;
    private bool isFacingRight;
    private Vector2 stuckDirection;
    private bool shouldNotFlipSprite;

    new void Start()
    {
        base.Start();
        provoked = false;
        fleeing = false;
        destinationReached = false;
        isRushing = false;
        rushCoolDownRemaining = 0;
        animator.SetInteger("RushStep", 0);
    }

    new void Update()
    {
        base.Update();
        UpdateVisuals();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if (!isRushing)
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

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        pathNeedUpdate = false;
        rangeFromInitialPos = Vector2.Distance(transform.position, initialPos);

        if (provoked && inControl)
        {
            fleeing = distToPlayer < safeDistanceToPlayer && rushCoolDownRemaining > 0;


            if (rushCoolDownRemaining <= 0 && IsPlayerInSightFrom(transform.position))
            {
                destinationReached = distToPlayer < rushTriggerDistance;

                if (distToPlayer < rushTriggerDistance)
                {
                    rushTriggerTimeElapsed += Time.deltaTime;
                    animator.SetInteger("RushStep", 1);
                                            

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
                    animator.SetInteger("RushStep", 0);
                }
            }
            else
            {
                rushCoolDownRemaining -= Time.deltaTime;
                rushTriggerTimeElapsed = 0;
                animator.SetInteger("RushStep", 0);

                if (destinationReached != distToPlayer >= safeDistanceToPlayer && distToPlayer < safeDistanceToPlayer + safeDistanceWidth)
                {
                    pathNeedUpdate = true;
                }

                destinationReached = distToPlayer >= safeDistanceToPlayer && distToPlayer < safeDistanceToPlayer + safeDistanceWidth;
            }



            if (!fleeing)
            {
                targetPathfindingPosition = GameData.player.transform.position;
            }
            else
            {
                Vector2 playerOppositeDirection = -playerDirection;
                playerOppositeDirection.Normalize();
                targetPathfindingPosition = (Vector2)transform.position + playerOppositeDirection * 3;
            }

            if (pathNeedUpdate)
            {
                UpdatePath();
            }
        }
        else
        {
            if(!isRushing)
            {
                targetPathfindingPosition = initialPos;
                destinationReached = Vector2.Distance(transform.position, initialPos) < 1;
                animator.SetInteger("RushStep", 0);
                rushTriggerTimeElapsed = 0;
            }
        }
    }

    private IEnumerator Rush(Vector2 rushDirection)
    {
        rushCoolDownRemaining = rushCooldown;
        rushTriggerTimeElapsed = 0;
        inControl = false;
        isRushing = true;

        //Fx de prévisualitation
        for (int i = 0; i < numberOfPrevisFx; i++)
        {
            GameObject previsClone = Instantiate(previsFX, (Vector2)transform.position + rushDirection * i * (rushLength / numberOfPrevisFx), Quaternion.identity);
            previsClone.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.left + Vector2.down, rushDirection));
            //previsClone.transform.localScale = new Vector3(1, rushDirection.x < 0 ? 1 : -1, 1);
        }

        yield return new WaitForSeconds(rushDelay);
        shouldNotFlipSprite = true;
        isProtected = true;

        Vector2 rushStartPos = transform.position;
        Vector2 dashEndPos = (Vector2)transform.position + rushDirection * rushLength;
        Vector2 rushPos = transform.position;
        Vector2 previousRushPos = transform.position;
        float currentRushSpeed;
        bool hasHit = false;
        bool hitWall = false;
        //transform.rotation = Quaternion.Euler(0, 0, rushDirection.x < 0 ? Vector2.SignedAngle(new Vector2(-1, -1), rushDirection) : Vector2.SignedAngle(new Vector2(1, -1), rushDirection));


        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.left + Vector2.down, rushDirection));

        float dashTimeElapsed = 0;
        while (dashTimeElapsed < rushTime && !isStuckInWall)
        {
            animator.SetInteger("RushStep", 2);
            dashTimeElapsed += Time.fixedDeltaTime;
            rushPos = Vector2.LerpUnclamped(rushStartPos, dashEndPos, rushCurve.Evaluate(dashTimeElapsed / rushTime));
            currentRushSpeed = (rushPos - previousRushPos).magnitude;
            previousRushPos = rushPos;
            rb.velocity = rushDirection * currentRushSpeed * (1 / Time.fixedDeltaTime);

            if (!hasHit)
            {
                hasHit = Physics2D.OverlapCircle(transform.position, rushRadius, LayerMask.GetMask("Player"));
                if (hasHit)
                {
                    GameData.playerManager.TakeDamage(1, rushDirection * rushKnockbackForce);
                }
                else
                {
                    hitWall = Physics2D.OverlapCircle(transform.position, rushWallRadius, LayerMask.GetMask("Wall", "DashWall"));
                    if (hitWall)
                    {
                        RaycastHit2D impactHit = Physics2D.CircleCast(transform.position, rushWallRadius, rushDirection, 2, LayerMask.GetMask("Wall", "DashWall"));
                        if(Vector2.Angle(-impactHit.normal, rushDirection) < 50)
                        {
                            isStuckInWall = true;
                            rb.constraints = RigidbodyConstraints2D.FreezeAll;
                            isProtected = false;
                            stuckDirection = impactHit.normal;
                            if (animator != null)
                            {
                                animator.SetBool("IsStuck", true);
                            }
                            Instantiate(stuckImpactParticle, transform.position, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, impactHit.normal)));
                            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.left + Vector2.down, -stuckDirection));
                        }
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
        if ((currentSheepShield != null && !currentSheepShield.isActive) || currentSheepShield == null)
        {
            isProtected = false;
        }

        animator.SetInteger("RushStep", 0);
        rb.velocity = Vector2.zero;
        if (isStuckInWall != true)
        {
            transform.rotation = Quaternion.identity;
            yield return new WaitForSeconds(rushStunTime);
            inControl = true;
        }
        else
        {
            yield return new WaitForSeconds(wallStunTime);
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.rotation = Quaternion.identity;
            inControl = true;
            isStuckInWall = false;
            if (animator != null)
            {
                animator.SetBool("IsStuck", false);
            }
        }
        isRushing = false;
        shouldNotFlipSprite = false;
    }

    private void UpdateVisuals()
    {
        if (isFacingRight && !sprite.flipX)
        {
            sprite.flipX = true;
        }
        if ((!isFacingRight || shouldNotFlipSprite) && sprite.flipX)
        {
            sprite.flipX = false;
        }

        animator.SetBool("IsFleeing", rushCoolDownRemaining > 0);
    }

    public override void DamageEffect()
    {

    }
}
