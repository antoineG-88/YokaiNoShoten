using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mante : Enemy
{
    [Header("Mante settings")]
    public float distanceToRetreat;
    public float fleeSpeed;
    public float maxDistanceToFlee;
    public GameObject firstPortal;
    public GameObject secondPortal;
    public float accelerationForce;
    public float stopDistance;
    public float playerDistanceToSabotagePortal;
    public float portalSabotageTime;
    public float sabotageStunTime;
    [Header("Cythe Attack settings")]
    public float cytheAttackCooldown;
    public float cytheAttackTeleguidingTime;
    public float cytheTimeBeforeSpin;
    public bool doCytheImmobilizeBeforeSpin;
    public float cytheSpinRadius;
    public GameObject cythe;
    public float cytheMaxSpeed;
    public float cytheBeforeSpinMaxSpeed;
    public float cytheMaxRecallSpeed;
    public float cytheAccelerationForce;
    public float cytheDistanceSpeedDampeningRatio;
    public float cytheTargetPlayerOffsetDistance;
    public float cytheKnockbackDistance;
    public LayerMask playerMask;
    public float timeBeforeFirstCytheThrow;
    [Header("Visuals")]
    public SpriteRenderer sprite;
    public GameObject disparitionFXPrefab;
    public GameObject retreatDirectionFXPrefab;
    public SpriteRenderer cytheAttackSprite;
    public float attackTransitionTime;
    public float previsState;
    public float cytheAttackCorrectedRadius;
    [Space]
    public float portalVisualDeactivationLerpSpeed;
    [Header("Temporary")]
    //public float baseCytheScale;
    //public float anticipationCytheScale;
    //public float cytheScaleLerpRatio;
    //public Color portalBaseColor;
    //public Color portalSabotagedColor;
    [Header("Mante specific Sounds")]
    public AudioSource cytheFlyingSoundSource;
    public Sound cytheFlyingSound;
    public Sound cytheFlyingBackSound;
    public Sound cytheSpinSound;
    public Sound teleportSound;
    public Sound cytheThrowSound;

    private bool isFleeing;
    private float playerDistToFirstPortal;
    private float playerDistToSecondPortal;
    private bool firstPortalSabotaged;
    private bool secondPortalSabotaged;
    private bool destinationReached;
    private float cytheCDElapsed;
    private float teleguidingTimeElapsed;
    private bool isTeleguidingCythe;
    private float cytheCurrentSpeed;
    private float cytheCurrentMaxSpeed;
    private Vector2 cytheTargetPos;
    private Vector2 cytheTargetDirection;
    private ContactFilter2D playerFilter;
    private bool isCytheInRecall;
    private bool isCytheSpinning;
    private float firstPortalCurrentVisualActivationRatio;
    private float secondPortalCurrentVisualActivationRatio;


    private float targetCytheScale;
    private Material cytheAttackMaterial;

    private SpriteRenderer firstPortalRenderer;
    private SpriteRenderer secondPortalRenderer;

    private Material firstPortalMat;
    private Material secondPortalMat;
    private bool isFacingRight;

    protected new void Start()
    {
        base.Start();
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(playerMask);
        firstPortalRenderer = firstPortal.transform.GetChild(0).GetComponent<SpriteRenderer>();
        secondPortalRenderer = secondPortal.transform.GetChild(0).GetComponent<SpriteRenderer>();
        cytheCDElapsed = cytheAttackCooldown - timeBeforeFirstCytheThrow;
        cytheAttackMaterial = Instantiate(cytheAttackSprite.sharedMaterial);
        cytheAttackSprite.sharedMaterial = cytheAttackMaterial;
        cytheAttackSprite.gameObject.transform.localScale = Vector2.one * cytheAttackCorrectedRadius * 2 * cytheSpinRadius;
        cytheAttackMaterial.SetFloat("_switchONOFF", 0);

        firstPortalMat = Instantiate(firstPortalRenderer.sharedMaterial);
        firstPortalRenderer.sharedMaterial = firstPortalMat;

        secondPortalMat = Instantiate(secondPortalRenderer.sharedMaterial);
        secondPortalRenderer.sharedMaterial = secondPortalMat;
    }

    protected new void Update()
    {
        base.Update();
        UpdateVisuals();
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateCythe();

        cytheFlyingSoundSource.pitch = Time.timeScale;
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        isFleeing = distToPlayer < maxDistanceToFlee;
        destinationReached = Vector2.Distance(transform.position, targetPathfindingPosition) < stopDistance;
        isFacingRight = playerDirection.x > 0;
        if (provoked)
        {
            if (inControl && !isDying)
            {
                if (isFleeing)
                {
                    if (Vector2.Distance((Vector2)transform.position - playerDirection, initialPos) < movementZoneRadius)
                    {
                        targetPathfindingPosition = (Vector2)transform.position - playerDirection;
                    }
                    else if(Vector2.Distance(GameData.player.transform.position, initialPos) < movementZoneRadius)
                    {
                        targetPathfindingPosition = GameData.player.transform.position;
                    }
                }
                else
                {
                    targetPathfindingPosition = transform.position;
                }


                CheckRetreat();
            }
        }
        else
        {
            targetPathfindingPosition = initialPos;
        }
    }

    private void UpdateCythe()
    {
        if(inControl && !isDying)
        {
            //cythe.transform.localScale = Vector2.one * Mathf.Lerp(cythe.transform.localScale.x, targetCytheScale, cytheScaleLerpRatio * Time.fixedDeltaTime);
            if (isTeleguidingCythe)
            {
                if (teleguidingTimeElapsed > cytheAttackTeleguidingTime)
                {
                    if (!isCytheSpinning)
                        StartCoroutine(SpinCythe());

                    if(!doCytheImmobilizeBeforeSpin)
                    {
                        cytheTargetPos = (Vector2)GameData.player.transform.position + GameData.movementHandler.rb.velocity.normalized * cytheTargetPlayerOffsetDistance;

                        cytheTargetDirection = cytheTargetPos - (Vector2)cythe.transform.position;
                        cytheTargetDirection.Normalize();

                        cytheCurrentMaxSpeed = Mathf.Clamp(Vector2.Distance(cythe.transform.position, cytheTargetPos) * cytheDistanceSpeedDampeningRatio, 0, cytheBeforeSpinMaxSpeed);
                        cytheCurrentSpeed += Time.fixedDeltaTime * cytheAccelerationForce;
                        cytheCurrentSpeed = Mathf.Clamp(cytheCurrentSpeed, 0, cytheCurrentMaxSpeed);

                        if (Vector2.Distance(cythe.transform.position, cytheTargetPos) > cytheCurrentSpeed * Time.fixedDeltaTime + 0.1f)
                        {
                            cythe.transform.position = (Vector2)cythe.transform.position + cytheTargetDirection * (cytheCurrentSpeed * Time.fixedDeltaTime);
                        }
                    }
                }
                else
                {
                    teleguidingTimeElapsed += Time.fixedDeltaTime;
                    /*
                    if(!isCytheSpinning)
                        targetCytheScale = Mathf.Lerp(baseCytheScale, anticipationCytheScale, teleguidingTimeElapsed / cytheAttackTeleguidingTime);*/
                        

                    cytheTargetPos = (Vector2)GameData.player.transform.position + GameData.movementHandler.rb.velocity.normalized * cytheTargetPlayerOffsetDistance;

                    cytheTargetDirection = cytheTargetPos - (Vector2)cythe.transform.position;
                    cytheTargetDirection.Normalize();

                    cytheCurrentMaxSpeed = Mathf.Clamp(Vector2.Distance(cythe.transform.position, cytheTargetPos) * cytheDistanceSpeedDampeningRatio, 0, cytheMaxSpeed);
                    cytheCurrentSpeed += Time.fixedDeltaTime * cytheAccelerationForce;
                    cytheCurrentSpeed = Mathf.Clamp(cytheCurrentSpeed, 0, cytheCurrentMaxSpeed);

                    if (Vector2.Distance(cythe.transform.position, cytheTargetPos) > cytheCurrentSpeed * Time.fixedDeltaTime + 0.1f)
                    {
                        cythe.transform.position = (Vector2)cythe.transform.position + cytheTargetDirection * (cytheCurrentSpeed * Time.fixedDeltaTime);
                    }
                }
            }
            else
            {
                if (isCytheInRecall)
                {
                    cytheTargetPos = transform.position;

                    cytheTargetDirection = cytheTargetPos - (Vector2)cythe.transform.position;
                    cytheTargetDirection.Normalize();
                    cytheCurrentSpeed += Time.fixedDeltaTime * cytheAccelerationForce;
                    cytheCurrentSpeed = Mathf.Clamp(cytheCurrentSpeed, 0, cytheMaxRecallSpeed);

                    if (Vector2.Distance(cythe.transform.position, cytheTargetPos) > cytheCurrentSpeed * Time.fixedDeltaTime + 0.1f)
                    {
                        cythe.transform.position = (Vector2)cythe.transform.position + cytheTargetDirection * (cytheCurrentSpeed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        isCytheInRecall = false;
                        cythe.SetActive(false);
                        cytheFlyingSoundSource.Stop();
                    }
                }
                else
                {
                    if (cytheCDElapsed > cytheAttackCooldown)
                    {
                        if (provoked && !isFleeing)
                        {
                            ThrowCythe();
                        }
                    }
                    else
                    {
                        cytheCDElapsed += Time.fixedDeltaTime;
                        teleguidingTimeElapsed = 0;
                    }
                }
            }
        }
        else
        {
            isTeleguidingCythe = false;
            isCytheInRecall = false;
        }
    }

    private void ThrowCythe()
    {
        cythe.SetActive(true);
        isTeleguidingCythe = true;
        teleguidingTimeElapsed = 0;
        cytheCurrentSpeed = 0;
        cythe.transform.position = transform.position;
        cytheAttackMaterial.SetFloat("_switchONOFF", 0);
        cytheFlyingSoundSource.clip = cytheFlyingSound.clip;
        cytheFlyingSoundSource.volume = cytheFlyingSound.volumeScale;
        cytheFlyingSoundSource.Play();
        source.PlayOneShot(cytheThrowSound.clip, cytheThrowSound.volumeScale);
        //cythe.transform.localScale = Vector2.one * baseCytheScale;
    }

    private IEnumerator SpinCythe()
    {
        //targetCytheScale = baseCytheScale;
        cytheAttackMaterial.SetFloat("_switchONOFF", previsState);

        cytheCDElapsed = 0;
        isCytheSpinning = true;
        if(doCytheImmobilizeBeforeSpin)
        {
            isTeleguidingCythe = false;
        }
        yield return new WaitForSeconds(cytheTimeBeforeSpin);

        cytheFlyingSoundSource.Stop();
        source.PlayOneShot(cytheSpinSound.clip, cytheSpinSound.volumeScale);

        targetCytheScale = cytheSpinRadius;
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCircle(cythe.transform.position, cytheSpinRadius, playerFilter, colliders);
        if(colliders.Count > 0)
        {
            cytheTargetDirection = (Vector2)GameData.player.transform.position - (Vector2)cythe.transform.position;
            cytheTargetDirection.Normalize();

            GameData.playerManager.TakeDamage(1, cytheTargetDirection * cytheKnockbackDistance);
        }
        if (!doCytheImmobilizeBeforeSpin)
        {
            isTeleguidingCythe = false;
        }

        cytheAttackMaterial.SetFloat("_switchONOFF", 0);
        float timer = 0;
        while(timer < attackTransitionTime)
        {
            cytheAttackMaterial.SetFloat("_switchONOFF", timer / attackTransitionTime);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //targetCytheScale = baseCytheScale;
        isCytheInRecall = true;

        cytheFlyingSoundSource.clip = cytheFlyingBackSound.clip;
        cytheFlyingSoundSource.volume = cytheFlyingBackSound.volumeScale;
        cytheFlyingSoundSource.Play();

        isCytheSpinning = false;
        cytheAttackMaterial.SetFloat("_switchONOFF", 0);
    }

    private void CheckRetreat()
    {
        playerDistToFirstPortal = Vector2.Distance(firstPortal.transform.position, GameData.player.transform.position);
        playerDistToSecondPortal = Vector2.Distance(secondPortal.transform.position, GameData.player.transform.position);

        if (!isTeleguidingCythe && !isCytheInRecall && !isCytheSpinning)
        {
            if (distToPlayer < distanceToRetreat)
            {

                if (playerDistToFirstPortal > playerDistToSecondPortal)
                {
                    firstPortalCurrentVisualActivationRatio = 2;
                    StartCoroutine(Retreat(firstPortal));
                }
                else
                {
                    secondPortalCurrentVisualActivationRatio = 2;
                    StartCoroutine(Retreat(secondPortal));
                }
            }
        }

        if(GameData.playerManager.isGrabbingTorch > 0)
        {
            if (playerDistToFirstPortal < playerDistanceToSabotagePortal && !firstPortalSabotaged)
            {
                StartCoroutine(SabotagePortal(1));
            }
            if (playerDistToSecondPortal < playerDistanceToSabotagePortal && !secondPortalSabotaged)
            {
                StartCoroutine(SabotagePortal(2));
            }
        }
    }

    private IEnumerator Retreat(GameObject portal)
    {
        source.PlayOneShot(teleportSound.clip, teleportSound.volumeScale);

        if (GameData.grappleHandler.attachedObject == gameObject)
        {
            GameData.grappleHandler.BreakRope("Mante tp");
        }

        if((portal == firstPortal && firstPortalSabotaged) || (portal == secondPortal && secondPortalSabotaged))
        {
            StartCoroutine(NoControl(sabotageStunTime));
        }

        Transform newFx = Instantiate(disparitionFXPrefab, transform.position, Quaternion.identity).transform;
        newFx.localScale = new Vector3(isFacingRight ? -1 : 1, 1, 1);
        Instantiate(retreatDirectionFXPrefab, transform.position, Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, portal.transform.position - transform.position)));

        transform.position = portal.transform.position;

        animator.SetTrigger("Teleport");

        yield return null;
    }

    public override void UpdateMovement()
    {
        if(inControl && !isDying)
        {
            if (path != null && !pathEndReached && inControl)
            {
                Vector2 force = new Vector2(pathDirection.x * accelerationForce, pathDirection.y * accelerationForce);

                rb.velocity += force * Time.fixedDeltaTime;

                if (rb.velocity.magnitude > fleeSpeed)
                {
                    rb.velocity = rb.velocity.normalized * fleeSpeed;
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
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override void DamageEffect()
    {
        cythe.SetActive(false);
    }

    private IEnumerator SabotagePortal(int portalIndex)
    {
        if(portalIndex == 1)
        {
            firstPortalSabotaged = true;
            //firstPortalRenderer.color = portalSabotagedColor;
        }
        else
        {
            secondPortalSabotaged = true;
            //secondPortalRenderer.color = portalSabotagedColor;
        }
        yield return new WaitForSeconds(portalSabotageTime);

        if (portalIndex == 1)
        {
            firstPortalSabotaged = false;
            //firstPortalRenderer.color = portalBaseColor;
        }
        else
        {
            secondPortalSabotaged = false;
            //secondPortalRenderer.color = portalBaseColor;
        }
    }

    private void UpdateVisuals()
    {
        if (isFacingRight && !sprite.flipX)
        {
            sprite.flipX = true;
        }
        if (!isFacingRight&& sprite.flipX)
        {
            sprite.flipX = false;
        }

        animator.SetBool("IsTeleguiding", isTeleguidingCythe || isCytheSpinning || isCytheInRecall);

        firstPortalCurrentVisualActivationRatio = Mathf.Lerp(firstPortalCurrentVisualActivationRatio, 0, portalVisualDeactivationLerpSpeed * Time.deltaTime);
        secondPortalCurrentVisualActivationRatio = Mathf.Lerp(secondPortalCurrentVisualActivationRatio, 0, portalVisualDeactivationLerpSpeed * Time.deltaTime);

        firstPortalMat.SetFloat("_idleOrActivated", Mathf.Clamp01(firstPortalCurrentVisualActivationRatio));
        secondPortalMat.SetFloat("_idleOrActivated", Mathf.Clamp01(secondPortalCurrentVisualActivationRatio));
    }

    protected override void OnDie()
    {
        base.OnDie();
        firstPortal.SetActive(false);
        secondPortal.SetActive(false);
        cytheFlyingSoundSource.Stop();
    }
}
