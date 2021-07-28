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
    private bool isProvoked;
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

    }
    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        isProvoked = Vector2.Distance(GameData.player.transform.position, initialPos) < provocationRange;
        destinationReached = safeDistanceToPlayer < distToPlayer;
        if (isProvoked)
        {
            if (!destinationReached)
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
    void CreateShield()
    {
        for (int i = 0; i < protectedEnemies.Length; i++)
        {
            SheepShield sheepShield = Instantiate(shield, protectedEnemies[i].transform);
            sheepShield.enemy = protectedEnemies[i];
            protectedEnemies[i].currentSheepShield = sheepShield;
        }
    }
    protected override void OnDie()
    {
        base.OnDie();
        for (int i = 0; i < protectedEnemies.Length; i++)
        {
            protectedEnemies[i].currentSheepShield.Disabling();
        }
    }
}
