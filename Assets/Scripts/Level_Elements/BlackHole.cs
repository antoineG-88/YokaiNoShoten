using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Piercable
{
    public float kbForce;
    public float suckingRadius;
    public float attackRadius;
    public AnimationCurve suckingForceByDistance;
    public float maxSuckingForce;
    public float deccelerationForce;
    public float suckEffectLerpSpeed;
    public float unsuckEffectLerpSpeed;
    public SpriteRenderer vortexSprite;
    public Animator centerAnimator;
    public Sound suckInSound;
    public Sound suckOutSound;
    public AudioSource suckLoopSource;
    public AudioSource source;

    private float succionPower;
    private bool playerInRange;
    private bool isInCenter;

    private Material vortexMat;
    private LayerMask playerMask;
    private float smoothedSuckEffect;


    void Start()
    {
        vortexMat = Instantiate(vortexSprite.sharedMaterial);
        vortexSprite.material = vortexMat;
        playerMask = LayerMask.GetMask("Player");
    }
    void Update()
    {
        if (playerInRange == true)
        {
            succionPower = suckingForceByDistance.Evaluate(1 - (Vector2.Distance(GameData.player.transform.position, transform.position) / suckingRadius)) * maxSuckingForce;
            //GameData.grappleHandler.BreakRope("nope");
            Vector2 dir = transform.position - GameData.movementHandler.transform.position;
            dir.Normalize();

            if (GameData.dashHandler.isDashing != true)
            {
                GameData.movementHandler.Propel(-GameData.movementHandler.rb.velocity * deccelerationForce * Time.deltaTime, false);
                GameData.movementHandler.Propel(dir * succionPower * Time.deltaTime, false);
            }

            isInCenter = Vector2.Distance(GameData.movementHandler.transform.position, transform.position) > attackRadius;

            UpdateAttack();
        }

        smoothedSuckEffect = Mathf.Lerp(smoothedSuckEffect, playerInRange ? 1 : 0, Time.deltaTime * (playerInRange ? suckEffectLerpSpeed : unsuckEffectLerpSpeed));
        vortexMat.SetFloat("SuckingValue", smoothedSuckEffect);
        vortexMat.SetFloat("SuckingSpin", vortexMat.GetFloat("SuckingSpin") + Time.deltaTime * 3f * smoothedSuckEffect);
        centerAnimator.SetBool("IsActivated", playerInRange);
    }

    private void UpdateAttack()
    {
        if (isInCenter)
        {
            if (Physics2D.OverlapCircle(transform.position, attackRadius, playerMask) != null)
            {
                GameData.dashHandler.isDashing = false;
                GameData.playerManager.TakeDamage(1, ((Vector2)GameData.movementHandler.transform.position - (Vector2)transform.position).normalized * kbForce);
            }
        }
    }

    void FixedUpdate()
    {
        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (Vector2.Distance(transform.position, GameData.player.transform.position) < suckingRadius)
        {
            if (playerInRange == false)
            {
                playerInRange = true;
                GameData.movementHandler.levitateSourceNumber++;
                suckLoopSource.Play();
                source.PlayOneShot(suckInSound.clip, suckInSound.volumeScale);
                //GameData.grappleHandler.isSucked = true;
            }
        }
        else
        {
            if (playerInRange == true)
            {
                playerInRange = false;
                GameData.movementHandler.levitateSourceNumber--;
                suckLoopSource.Stop();
                //source.PlayOneShot(suckOutSound.clip, suckOutSound.volumeScale);
                //GameData.grappleHandler.isSucked = false;
            }
        }
    }

    private void SetFree()
    {
        //GetComponent<SpriteRenderer>().color = Color.red;

        GameData.movementHandler.levitateSourceNumber--;
    }


    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        //SetFree();
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, suckingRadius);
    }
}
