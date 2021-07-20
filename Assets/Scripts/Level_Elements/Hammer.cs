using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    Rigidbody2D rb;
    public float resetPositionTimer;
    public float downSurge;
    public float upSurge;
    public float kbForce;
    private Vector2 formerPosition;
    private bool hitFlag;
    public LayerMask playerLayer;
    private ContactFilter2D playerFilter;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        formerPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && hitFlag == false)
        {
            Slam(collision);
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameData.playerManager.TakeDamage(1, (collision.transform.position - transform.position) * kbForce);
        }
    }*/
    void Slam(Collider2D collider)
    {
        //hitFlag = true;
        //StartCoroutine(ResetPosition(resetPositionTimer));
        rb.velocity = Vector2.down.normalized * downSurge;
        List<Collider2D> hitPlayer = new List<Collider2D>();
        Physics2D.OverlapCollider(collider, playerFilter, hitPlayer);
        {

        }
    }

    /*IEnumerator ResetPosition(float resetPosition)
    {
        yield return new WaitForSeconds(resetPosition);

        do
        {
            Debug.Log("position:" + transform.position.y);
            yield return new WaitForFixedUpdate();
            Debug.Log("former position:" + formerPosition.y);
            rb.velocity = Vector2.up.normalized * upSurge;
        }
        while (transform.position.y <= formerPosition.y);
        rb.velocity = Vector2.zero;

        hitFlag = false;
        yield return new WaitForSeconds(2f);
        Debug.Log(transform.position.y);
    }*/
}
