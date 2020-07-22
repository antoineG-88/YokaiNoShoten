using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpent : Enemy
{
    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float turningBackDistance;
    public float provocationRange;
    [Header("Bomb settings")]
    public float timeBetweenBombDrop;
    public GameObject bombPrefab;
    [Header("Technical settings")]
    public List<GameObject> bodyParts;

    private bool movingRight;
    private float timeBeforeNextBombDrop;

    private new void Start()
    {
        base.Start();
        provoked = false;
        timeBeforeNextBombDrop = timeBetweenBombDrop;
    }

    private new void Update()
    {
        base.Update();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if (provoked)
        {
            rb.velocity = new Vector2(rb.velocity.x + (movingRight ? accelerationForce : -accelerationForce) * Time.fixedDeltaTime, 0);
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
        else
        {
            provoked = distToPlayer < provocationRange;
        }
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();

        if(timeBeforeNextBombDrop > 0)
        {
            timeBeforeNextBombDrop -= Time.deltaTime;
        }
        else
        {
            timeBeforeNextBombDrop = timeBetweenBombDrop;
            DropBomb();
        }

        if (Physics2D.Raycast(transform.position, movingRight ? Vector2.right : Vector2.left, turningBackDistance, LayerMask.GetMask("Wall")))
        {
            movingRight = !movingRight;
        }
    }

    private void DropBomb()
    {
        Instantiate(bombPrefab, transform.position, Quaternion.identity);
    }
}
