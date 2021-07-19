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
            Slam();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameData.playerManager.TakeDamage(1, (collision.transform.position - transform.position) * kbForce);
        }
    }
    void Slam()
    {
        hitFlag = true;
        rb.velocity = Vector2.down.normalized * downSurge;
        StartCoroutine(ResetPosition(resetPositionTimer));
    }
    IEnumerator ResetPosition(float resetPosition)
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
    }
}
