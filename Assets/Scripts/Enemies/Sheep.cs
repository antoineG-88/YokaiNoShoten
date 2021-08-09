using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Enemy
{
    public Enemy[] protectedEnemies;
    public SheepShield shield;

    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float safeDistanceToPlayer;

    private bool isFacingRight;
    private bool destinationReached;
    private bool isProvoked;
    private bool isFleeing;

    new void Start()
    {
        base.Start();
        CreateShield();
    }

    new void Update()
    {
        base.Update();
    }
    new void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void UpdateMovement()
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

    public override void DamageEffect()
    {

    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        //isProvoked = Vector2.Distance(GameData.player.transform.position, initialPos) < provocationRange && Vector2.Distance(transform.position, initialPos) < movementZoneRadius;
        destinationReached = Vector2.Distance(targetPathfindingPosition, transform.position) < 0.5f;
        isFleeing = distToPlayer < safeDistanceToPlayer;
        if (isProvoked)
        {
            if (isFleeing)
            {
                targetPathfindingPosition = (Vector2)transform.position - playerDirection * 2;
            }
            else
            {
                targetPathfindingPosition = transform.position;
            }
        }
        else
        {
            targetPathfindingPosition = initialPos;
        }
    }

    private void CreateShield()
    {
        for (int i = 0; i < protectedEnemies.Length; i++)
        {
            if (protectedEnemies[i] != null && protectedEnemies[i].gameObject.activeSelf)
            {
                SheepShield sheepShield = Instantiate(shield, protectedEnemies[i].transform);
                sheepShield.enemy = protectedEnemies[i];
                sheepShield.connectedSheep = this;
                sheepShield.isActive = true;
                protectedEnemies[i].currentSheepShield = sheepShield;
            }
        }
    }

    protected override void OnDie()
    {
        base.OnDie();
        for (int i = 0; i < protectedEnemies.Length; i++)
        {
            if(protectedEnemies[i] != null && protectedEnemies[i].gameObject.activeSelf)
                protectedEnemies[i].currentSheepShield.Disabling();
        }
    }
}
