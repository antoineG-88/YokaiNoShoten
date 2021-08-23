using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    Vector2 directedForce;
    public float surge;
    public float centerSurge;
    public Transform bumperDirection;
    private bool canPush;
    private bool isEntered;
    private Vector2 centerForce;

    void Start()
    {
        directedForce = bumperDirection.position - transform.position;
        directedForce.Normalize();
        directedForce *= surge;
    }

    void Update()
    {
        directedForce = bumperDirection.position - transform.position;
        directedForce.Normalize();
        directedForce *= surge;
        if (isEntered)
        {
            centerForce = transform.position - GameData.movementHandler.transform.position;
            centerForce.Normalize();
            centerForce *= centerSurge;
            GameData.movementHandler.rb.velocity = centerForce;
            GameData.playerManager.inControl = false;
        }

        if (Vector2.Distance(GameData.movementHandler.transform.position, transform.position) <= (Time.deltaTime * centerSurge * 3) && canPush == true)
        {
            canPush = false;
            isEntered = false;
            GameData.movementHandler.Propel(directedForce, true);
            GameData.playerManager.inControl = true;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isEntered = true;
            GameData.grappleHandler.BreakRope("nope");
            canPush = true;
            //Play gameObject Animation & SFX
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (bumperDirection.position - transform.position).normalized * 10f);
    }
}