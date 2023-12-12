using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : Enemy
{
    [Header("Sentinel options")]
    public GameObject shield;
    public SpriteRenderer sprite;
    public LinkSwitch waveReceiver;
    public LinkSwitch shieldSwitch;
    [Header("Sentinel protection")]
    public GameObject protectionLinkPrefab;
    [HideInInspector] public List<Enemy> protectingEnemies;
    [HideInInspector] public List<SentinelProtectionLink> protectingLinks;
    public List<SentinelProtection> protections;
    public float vulnerabilityTimeWindow;
    public AnimationCurve shieldApparitionCurve;
    public float shieldApparitionTime = 0.3f;
    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float deccelerationForce;
    public float safeDistanceToPlayer;
    public float movementSlowingRatio;
    public float playerPropelForceMultiplier;
    public Transform restPos1; 

    private bool destinationReached;
    private bool isFacingRight;
    private bool isShielded;
    private bool isDefending;
    private float timeElapsedWithoutShield;
    private int currentProtectionLevel;
    private bool lateStartFlag;
    private float shieldBaseLocalScale;

    new void Start()
    {
        base.Start();
        shieldBaseLocalScale = shield.transform.localScale.x;
        shield.SetActive(false);
        ActivateShield();
        waveReceiver.isOn = false;
        shieldSwitch.isOn = false;
        isDefending = false;
        currentProtectionLevel = 0;
        lateStartFlag = true;
        isFacingRight = true;
    }

    new void Update()
    {
        base.Update();

        if (lateStartFlag)
        {
            lateStartFlag = false;
            LateStart();
        }

        UpdateVisuals();
        if(!isDefending && waveReceiver.IsON())
        {
            currentProtectionLevel++;
            isDefending = true;
            ActivateShield();
            StartProtection(protections[Mathf.Min(currentProtectionLevel - 1, protections.Count - 1)].protectingEnemies);
            waveReceiver.isOn = false;
        }
    }

    private void LateStart()
    {
        for (int i = 0; i < protections.Count; i++)
        {
            for (int y = 0; y < protections[i].protectingEnemies.Count; y++)
            {
                protections[i].protectingEnemies[y].Deactivate();
            }
        }
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if(isShielded)
        {
            isFacingRight = pathDirection.x < 0;
            if (path != null && !pathEndReached && !destinationReached && inControl && !isDying)
            {
                Vector2 force = new Vector2(pathDirection.x, pathDirection.y);

                rb.velocity += force * Time.fixedDeltaTime * accelerationForce;

                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }
            else if (destinationReached)
            {
                rb.velocity -= rb.velocity.normalized * deccelerationForce * Time.fixedDeltaTime;
                if (rb.velocity.magnitude <= deccelerationForce * Time.fixedDeltaTime)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
        rb.velocity *= movementSlowingRatio;
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();

        if(isDefending)
        {
            targetPathfindingPosition = (Vector2)transform.position - playerDirection.normalized * 5;

            destinationReached = distToPlayer >= safeDistanceToPlayer;
        }
        else
        {
            targetPathfindingPosition = restPos1.position;
            destinationReached = Vector2.Distance(transform.position, restPos1.position) < 0.2f;
        }

        UpdateProtection();
    }

    private void UpdateProtection()
    {
        if(isDefending)
        {
            if(isShielded)
            {
                bool shouldRemoveProtection = true;

                for (int i = 0; i < protectingEnemies.Count; i++)
                {
                    if (protectingEnemies[i] != null)
                    {
                        if(protectingEnemies[i].isDying)
                        {
                            protectingLinks[i].Disabling();
                        }
                        else
                        {
                            shouldRemoveProtection = false;
                        }
                    }
                }

                if(shouldRemoveProtection)
                {
                    DisableShield();
                }
                timeElapsedWithoutShield = 0;
            }
            else
            {
                timeElapsedWithoutShield += Time.deltaTime;
                if(timeElapsedWithoutShield > vulnerabilityTimeWindow)
                {
                    ActivateShield();
                    StartProtection(protections[Mathf.Min(currentProtectionLevel - 1, protections.Count - 1)].protectingEnemies);
                }
            }
        }
    }

    private void StartProtection(List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject newEnemyObject = Instantiate(enemies[i].prefabObject, enemies[i].transform.position, Quaternion.identity, transform);
            Enemy newEnemy = newEnemyObject.GetComponent<Enemy>();
            if(newEnemy == null)
            {
                newEnemy = newEnemyObject.transform.GetChild(0).GetComponent<Enemy>();
            }
            StartCoroutine(newEnemy.Activate());
            protectingEnemies.Add(newEnemy);
            SentinelProtectionLink protectionLink = Instantiate(protectionLinkPrefab, newEnemy.transform).GetComponent<SentinelProtectionLink>();
            protectionLink.enemy = newEnemy;
            protectionLink.connectedSentinel = this;
            protectionLink.isActive = true;
            protectingLinks.Add(protectionLink);
        }
    }

    private void UpdateVisuals()
    {
        if (isFacingRight && !sprite.flipX)
        {
            sprite.flipX = true;
            sprite.transform.localPosition = new Vector2(0.2f, 0.04f);
        }
        if (!isFacingRight && sprite.flipX)
        {
            sprite.flipX = false;
            sprite.transform.localPosition = new Vector2(-0.2f, 0.04f);
        }
    }

    protected override void OnDie()
    {
        rb.velocity = Vector2.zero;
    }

    private void ActivateShield()
    {
        if(!isShielded)
        {
            StartCoroutine(ShieldActivationEffect());
        }
        isShielded = true;
        animator.SetBool("IsShielded", true);
        shield.SetActive(true);
    }

    private IEnumerator ShieldActivationEffect()
    {
        float timer = 0;
        while(timer < shieldApparitionTime)
        {
            shield.transform.localScale = Vector3.one * Mathf.Lerp(0, shieldBaseLocalScale, shieldApparitionCurve.Evaluate(timer / shieldApparitionTime));
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        shield.transform.localScale = Vector3.one * shieldBaseLocalScale;
    }

    private IEnumerator ShieldDeactivationEffect()
    {
        float timer = 0;
        while (timer < shieldApparitionTime)
        {
            shield.transform.localScale = Vector3.one * Mathf.Lerp(shieldBaseLocalScale, 0, shieldApparitionCurve.Evaluate(timer / shieldApparitionTime));
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        shield.transform.localScale = Vector3.zero;

        shield.SetActive(false);
    }

    private IEnumerator DelayedShieldActivation()
    {
        yield return new WaitForSeconds(0.5f);
        ActivateShield();
    }

    private void DisableShield()
    {
        isShielded = false;
        animator.SetBool("IsShielded", false);
        protectingEnemies.Clear();
        protectingLinks.Clear();
        StartCoroutine(ShieldDeactivationEffect());
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        if (!isProtected && !isShielded)
        {
            triggerSlowMo = true;
            TakeDamage(damage, 0.5f);
        }
        else
        {
            triggerSlowMo = false;
            if (currentSheepShield != null && currentSheepShield.isActive)
            {
                source.PlayOneShot(shieldHitSound.clip, shieldHitSound.volumeScale);
            }

            Propel(directedForce * playerPropelForceMultiplier);
            StartCoroutine(NoControl(0.5f));
        }
        return isShielded;
    }

    public override void DamageEffect()
    {
        animator.SetTrigger("Hurt");
        isDefending = false;
        StartCoroutine(DelayedShieldActivation());
        StartCoroutine(StartTempShieldSwitchActivation());
    }

    private IEnumerator StartTempShieldSwitchActivation()
    {
        yield return new WaitForSeconds(1.0f);
        shieldSwitch.isOn = true;
        yield return new WaitForSeconds(5.0f);
        shieldSwitch.isOn = false;
    }

    [System.Serializable]
    public class SentinelProtection
    {
        public List<Enemy> protectingEnemies;
    }
}
