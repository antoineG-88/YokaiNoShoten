using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritPart : MonoBehaviour
{
    public float drag;
    public float pickupDistance;
    public float pickupMagnetismDistance;
    public float pickupSpeed;
    public float timeBeforePickable;

    [HideInInspector] public int pointsHeld;
    [HideInInspector] public float timeRemainingBeforePickable;

    private Rigidbody2D rb;
    private float distanceToPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int points, Vector2 initialVelocity)
    {
        rb.velocity = initialVelocity;
        pointsHeld = points;
        timeRemainingBeforePickable = timeBeforePickable;
    }

    private void Update()
    {
        if(timeRemainingBeforePickable > 0)
        {
            timeRemainingBeforePickable -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        TestPickup();
    }

    private void UpdateMovement()
    {
        rb.velocity -= rb.velocity.normalized * drag * Time.fixedDeltaTime;
        distanceToPlayer = Vector2.Distance(transform.position, GameData.player.transform.position);
        if(distanceToPlayer < pickupMagnetismDistance && timeRemainingBeforePickable <= 0)
        {
            Vector2 playerDirection = GameData.player.transform.position - transform.position;
            playerDirection.Normalize();
            rb.velocity = playerDirection * pickupSpeed;
        }
    }

    private void TestPickup()
    {
        if(distanceToPlayer < pickupDistance && timeRemainingBeforePickable <= 0)
        {
            //GameData.playerManager.PickSpiritPart(this);
        }
    }
}
