using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    Vector2 directedForce;
    public float verticalSurge;
    public Transform bumperDirection;

    void Start()
    {
        directedForce = bumperDirection.position - transform.position;
        directedForce.Normalize();
        directedForce *= verticalSurge;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GameData.movementHandler.Propel(directedForce, true);
            //Play gameObject Animation & SFX
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (bumperDirection.position - transform.position).normalized*10f);
    }
}