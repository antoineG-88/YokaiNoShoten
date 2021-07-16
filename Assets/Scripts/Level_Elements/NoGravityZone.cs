using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityZone : MonoBehaviour
{
    private bool isInZone;
    private Serpent potentialSerpent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInZone = true;
        }

        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInZone = false;
        }


        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = false;
        }
    }

    private void Update()
    {
        GameData.movementHandler.isInNoGravityZone = isInZone;
    }
}
