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
    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float deccelerationForce;
    public float safeDistanceToPlayer;
    public Transform restPos1; 

    private bool destinationReached;
    private bool isFacingRight;
    private bool isShielded;

    new void Start()
    {
        base.Start();
        shield.SetActive(false);
        ActivateShield();
        waveReceiver.isOn = false;
        shieldSwitch.isOn = false;
    }

    new void Update()
    {
        base.Update();
        UpdateVisuals();
        if(isShielded && waveReceiver.IsON())
        {
            DisableShield();
            waveReceiver.isOn = false;
        }
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if (!isShielded)
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
        rb.velocity *= 0.85f;
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        /*
        targetPathfindingPosition = (Vector2)transform.position - playerDirection.normalized * 3;

        destinationReached = distToPlayer >= safeDistanceToPlayer;
        */
        targetPathfindingPosition = restPos1.position;
        destinationReached = Vector2.Distance(transform.position, restPos1.position) < 1;
    }

    private void UpdateVisuals()
    {
        if (isFacingRight && !sprite.flipX)
        {
            sprite.flipX = true;
        }
        if (!isFacingRight && sprite.flipX)
        {
            sprite.flipX = false;
        }
    }

    protected override void OnDie()
    {
        rb.velocity = Vector2.zero;
    }

    private void ActivateShield()
    {
        isShielded = true;
        animator.SetBool("IsShielded", true);
        shield.SetActive(true);
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
        shield.SetActive(false);
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

            Propel(directedForce * 3);
        }
        return isShielded;
    }

    public override void DamageEffect()
    {
        animator.SetTrigger("Hurt");
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
}
