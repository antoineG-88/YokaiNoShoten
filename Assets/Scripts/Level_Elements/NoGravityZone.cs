using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityZone : MonoBehaviour
{
    PlayerManager player;
    bool isInZone;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.GetComponent<PlayerManager>();
        if(player != null)
        {
            
            isInZone = true;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player = collision.GetComponent<PlayerManager>();
        if(player != null)
        {
            isInZone = false;
        }
        
    }

    private void Update()
    {
        
        if (isInZone == true)
        {
            GameData.movementHandler.isAffectedbyGravity = false;
            
        }
        else
        {
            GameData.movementHandler.isAffectedbyGravity = true;
        }
    }
}
