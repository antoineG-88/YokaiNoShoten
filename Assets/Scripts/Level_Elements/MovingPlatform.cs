using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool stabilizeRotation;
    public bool calculateVelocityBasedOnMovement;
    public Collider2D groundChangeCollider;
    ContactFilter2D playerfilter;
    List<Collider2D> playerCollider = new List<Collider2D>();
    Rigidbody2D rb;
    private float startRotation;
    private Vector2 previousPos;
    private Vector2 currentPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerfilter.SetLayerMask(LayerMask.GetMask("Player"));
        startRotation = transform.rotation.eulerAngles.z;
        currentPos = transform.position;
    }


    void FixedUpdate()
    {
        Physics2D.OverlapCollider(groundChangeCollider, playerfilter, playerCollider);
        if (playerCollider.Count > 0)
        {

            GameData.movementHandler.groundRb = rb;
        }
        else if(GameData.movementHandler.groundRb == rb )
        {
            GameData.movementHandler.groundRb = null;
        }

        if(stabilizeRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, startRotation);
        }

        if (calculateVelocityBasedOnMovement)
        {
            previousPos = currentPos;
            currentPos = transform.position;
            Vector2 velocity = currentPos - previousPos;
            velocity /= Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }
    private void LateUpdate()
    {
        if (stabilizeRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, startRotation);
        }
    }
}
