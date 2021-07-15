using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public Transform WaypointA;
    public Transform WaypointB;
    public float knockbackDirectedForce;
    public bool canMove;
    public float sawSpeed;
    public bool goesRight;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SawMovement();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (Vector2)(GameData.player.transform.position - gameObject.transform.position);
            GameData.playerManager.TakeDamage(1, direction.normalized * knockbackDirectedForce);
        }
    }

    private void SawMovement()
    {
        if (goesRight == true)
        {
            Vector2 directionB = (WaypointB.position - transform.position);
            rb.velocity = directionB.normalized * sawSpeed;
            if (Vector2.Distance(transform.position,WaypointB.position)<0.5f)
            {
                goesRight = false;
                Debug.Log("B");
            }
        }
        else
        {
            Vector2 directionA = (WaypointA.position - transform.position);
            rb.velocity = directionA.normalized * sawSpeed;
            if (Vector2.Distance(transform.position, WaypointA.position) < 0.5f)
            {
                goesRight = true;
            }
        }
    }
}
