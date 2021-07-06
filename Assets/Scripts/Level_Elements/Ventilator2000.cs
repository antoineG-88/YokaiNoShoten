using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilator2000 : MonoBehaviour
{
    public float overlapRange;
    ContactFilter2D playerFilter;
    List<Collider2D> playerCollider = new List<Collider2D>();
    public float aspirationForce;
    private Vector2 playerDirection;
    public float dragForce;
    public float dragPower;
    void Start()
    {
        playerFilter.SetLayerMask(LayerMask.GetMask ("Player"));
    }

    void Update()
    {
        Physics2D.OverlapCircle(transform.position, overlapRange, playerFilter, playerCollider);
        if (playerCollider.Count > 0)
        {
            GameData.movementHandler.rb.velocity -= GameData.movementHandler.rb.velocity.normalized * Mathf.Pow(GameData.movementHandler.rb.velocity.magnitude * dragForce, dragPower * 2);
            playerDirection = (transform.position - GameData.player.transform.position).normalized;
            //GameData.movementHandler.isAffectedbyGravity = false;
            GameData.movementHandler.Propel(playerDirection * aspirationForce, false);
           
        }
        else
        {
            //GameData.movementHandler.isAffectedbyGravity = true;
        }
    }
}
