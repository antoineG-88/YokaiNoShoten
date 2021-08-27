using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Piercable
{
    public float trappedTime;
    public float formerDisablingHoleTimer;
    public float kbForce;
    public float suckingRadius;
    public AnimationCurve suckingForceByDistance;
    public float maxSuckingForce;

    private float timer;
    private float disablingHoleTimer;
    private float succionPower;
    private bool isInRange;
    private bool hasAttacked;
    private Vector2 suckedPosition;
    private bool isDesactivated;
    private bool isInCenter;



    void Start()
    {
        disablingHoleTimer = formerDisablingHoleTimer;
    }
    void Update()
    {
        if (isInRange == true && hasAttacked == false && isDesactivated == false)
        {
            succionPower = suckingForceByDistance.Evaluate(1 - (Vector2.Distance(GameData.player.transform.position, transform.position) / suckingRadius)) * maxSuckingForce;
            GameData.grappleHandler.BreakRope("nope");
            Vector2 dir = transform.position - GameData.movementHandler.transform.position;
            dir.Normalize();

            if(Vector2.Distance(GameData.movementHandler.transform.position, transform.position) > succionPower * Time.fixedDeltaTime)
            {
                if (GameData.dashHandler.isDashing != true)
                {
                    GameData.movementHandler.rb.velocity = dir * succionPower;
                }
                isInCenter = false;
            }
            else
            {
                if(!isInCenter)
                {
                    GameData.movementHandler.rb.velocity = Vector2.zero;
                    GameData.player.transform.position = transform.position;
                    isInCenter = true;
                }
            }

            Attack();
        }
        if (isDesactivated == true)
        {
            disablingHoleTimer -= Time.deltaTime;
            if (disablingHoleTimer <= 0)
            {
                disablingHoleTimer = formerDisablingHoleTimer;
                isDesactivated = false;
            }
        }
    }
    void FixedUpdate()
    {
        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (Vector2.Distance(transform.position, GameData.player.transform.position) < suckingRadius && isDesactivated == false)
        {
            if (isInRange == false)
            {
                isInRange = true;
                GameData.movementHandler.levitateSourceNumber++;
                suckedPosition = GameData.movementHandler.transform.position;
                GameData.grappleHandler.isSucked = true;
            }
        }
        else
        {
            if (isInRange == true)
            {
                isInRange = false;
                GameData.movementHandler.levitateSourceNumber--;
                hasAttacked = false;
                GameData.grappleHandler.isSucked = false;
            }
        }
    }

    private void Attack()
    {
        if (Vector2.Distance(transform.position, GameData.movementHandler.transform.position) < 0.05f * succionPower)
        {
            GameData.pierceHandler.canPierce = false;
            timer += Time.deltaTime;
            if (timer > trappedTime)
            {
                GameData.playerManager.TakeDamage(1, (suckedPosition - (Vector2)transform.position).normalized * kbForce);
                timer = 0f;
                hasAttacked = true;
            }
        }
    }

    private void SetFree()
    {
        //GetComponent<SpriteRenderer>().color = Color.red;
        isDesactivated = true;
        GameData.movementHandler.levitateSourceNumber--;
    }


    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        SetFree();
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, suckingRadius);
    }
}
