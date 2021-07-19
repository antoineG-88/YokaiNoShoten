using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float distanceInfluence;
    public float basePower;
    public float succionPower;
    private bool isInRange;
    [Range(0, 15)]
    public float minValueSuccionPower;
    [Range(15, 30)]
    public float maxValueSuccionPower;

    void Start()
    {
    }
    void Update()
    {
        if (isInRange == true)
        {
            succionPower = basePower * (1 / (Vector2.Distance(GameData.movementHandler.transform.position, transform.position)*distanceInfluence));
            succionPower = Mathf.Clamp(succionPower, minValueSuccionPower, maxValueSuccionPower);
            GameData.grappleHandler.BreakRope("nope");
            Vector2 dir = (transform.position - GameData.movementHandler.transform.position).normalized;
            if (GameData.dashHandler.isDashing != true)
            {
                GameData.movementHandler.rb.velocity = dir * succionPower;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Player"))
        {
            isInRange = true;

            GameData.movementHandler.isInNoGravityZone = true;
            GameData.dashHandler.canDash = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Player"))
        {
            isInRange = false;
            GameData.movementHandler.isInNoGravityZone = false;
        }
    }
}
