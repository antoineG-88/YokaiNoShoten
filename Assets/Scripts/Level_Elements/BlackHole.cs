using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Piercable
{
    public float timer;
    public float disablingHoleTimer;
    public float formerDisablingHoleTimer;
    public float distanceInfluence;
    public float basePower;
    public float kbForce;
    private float succionPower;
    private bool isInRange;
    [Range(0, 15)]
    public float minValueSuccionPower;
    [Range(15, 30)]
    public float maxValueSuccionPower;
    private bool hasAttacked;
    private Vector2 suckedPosition;
    public Color startColor;
    public float suckingRadius;
    private bool isDesactivated;

    void Start()
    {
        disablingHoleTimer = formerDisablingHoleTimer;
        startColor = GetComponent<SpriteRenderer>().color;
    }
    void Update()
    {
        if (isInRange == true && hasAttacked == false && isDesactivated == false)
        {
            succionPower = basePower * (1f / (Vector2.Distance(GameData.movementHandler.transform.position, transform.position) * distanceInfluence));
            succionPower = Mathf.Clamp(succionPower, minValueSuccionPower, maxValueSuccionPower);
            GameData.grappleHandler.BreakRope("nope");
            Vector2 dir = (transform.position - GameData.movementHandler.transform.position).normalized;
            if (GameData.dashHandler.isDashing != true)
            {
                GameData.movementHandler.rb.velocity = dir * succionPower;
            }
            Attack();
        }
        if (isDesactivated == true)
        {
            disablingHoleTimer -= Time.deltaTime;
            if (disablingHoleTimer <= 0)
            {
                GetComponent<SpriteRenderer>().color = startColor;
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
                GameData.movementHandler.isInNoGravityZone = true;
                suckedPosition = GameData.movementHandler.transform.position;
                GameData.grappleHandler.isSucked = true;
            }
        }
        else
        {
            if (isInRange == true)
            {
                isInRange = false;
                GameData.movementHandler.isInNoGravityZone = false;
                hasAttacked = false;
                GameData.grappleHandler.isSucked = false;
            }
        }
    }

    private void Attack()
    {
        if (Vector2.Distance(transform.position, GameData.movementHandler.transform.position) < 0.2f)
        {
            GameData.pierceHandler.canPierce = false;
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                GameData.playerManager.TakeDamage(1, (suckedPosition - (Vector2)transform.position).normalized * kbForce);
                Debug.Log((suckedPosition - (Vector2)transform.position).normalized * kbForce);
                timer = 0f;
                hasAttacked = true;
            }
        }
    }
    private void SetFree()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        isDesactivated = true;
        GameData.movementHandler.isInNoGravityZone = false;
    }


    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        SetFree();
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, suckingRadius);
    }
}
