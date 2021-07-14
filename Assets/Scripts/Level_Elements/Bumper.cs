using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    Vector2 directedForce;
    public float verticalSurge;

    void Start()
    {
        directedForce = new Vector2(0, verticalSurge);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GameData.movementHandler.Propel(directedForce, true);
            //Play gameObject Animation & SFX
        }
    }
}