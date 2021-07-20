using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    Vector2 directedForce;
    public float surge;
    public Transform bumperDirection;
    public bool canPush;
    public bool isEntered;
    public Vector2 centerForce;

    void Start()
    {
        directedForce = bumperDirection.position - transform.position;
        directedForce.Normalize();
        directedForce *= surge;
    }
    void Update()
    {
        if(isEntered)
        {
            centerForce = transform.position - GameData.movementHandler.transform.position;
            centerForce.Normalize();
            centerForce *= surge;
            GameData.movementHandler.rb.velocity = centerForce;
        }
       if (Vector2.Distance(GameData.movementHandler.transform.position, transform.position) < (Time.deltaTime*surge) && canPush == true)
        {
            canPush = false;
            isEntered = false;
            GameData.movementHandler.Propel(directedForce, true);
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
        Gizmos.DrawRay(transform.position, (bumperDirection.position - transform.position).normalized*10f);
    }
}