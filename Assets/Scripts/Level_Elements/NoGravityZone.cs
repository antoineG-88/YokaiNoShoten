using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityZone : MonoBehaviour
{
    public float minSpeedInNGZone;
    public float momentumSlowingForce;
    public float maxSpeedInNGZone;
    public float aboveMaxMomentumSlowingForce;

    private Serpent potentialSerpent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameData.movementHandler.currentGravityZone = this;
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
            GameData.movementHandler.currentGravityZone = null;
        }


        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = false;
        }
    }
}
