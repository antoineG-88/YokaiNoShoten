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
    public float provocationRange;
    public float safeDistanceToPlayer;
    public float maxRangeFromInitialPos;
    private bool isFacingRight;
    private bool destinationReached;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        CreateShield();
    }

    // Update is called once per frame
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
        throw new System.NotImplementedException();
    }
    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        destinationReached = safeDistanceToPlayer < distToPlayer;
        if (!destinationReached)
        {
            targetPathfindingPosition = (Vector2)transform.position - playerDirection * 2;
        }
        else
        {
            targetPathfindingPosition = transform.position;
        }
    }
    void CreateShield()
    {
        for (int i = 0; i < protectedEnemies.Length; i++)
        {
            SheepShield sheepShield = Instantiate(shield, protectedEnemies[i].transform);
            sheepShield.enemy = protectedEnemies[i];
        }
    }
}
