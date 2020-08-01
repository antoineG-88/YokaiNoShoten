using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Collider2D groundChangeCollider;
    ContactFilter2D playerfilter;
    List<Collider2D> playerCollider = new List<Collider2D>();
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerfilter.SetLayerMask(LayerMask.GetMask("Player"));
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
        
    }
}
