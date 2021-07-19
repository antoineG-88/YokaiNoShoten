using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawablePlatform : MonoBehaviour
{
    public Transform rightPosition;
    public Transform leftPosition;
    public float speed;
    public DrawablePlatformRing rightRing;
    public DrawablePlatformRing leftRing;
    Rigidbody2D rb;
    Vector2 currentDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        UpdateDirection();
    }

    void UpdateDirection()
    {
        if (Vector2.Distance(transform.position, rightPosition.position) < 0.1f * speed && rightRing.moveRight == true)
        {
            transform.position = rightPosition.position;
            rightRing.moveRight = false;
            rb.velocity = Vector2.zero;
            
        }

        if (Vector2.Distance(transform.position, leftPosition.position) < 0.1f * speed && leftRing.moveLeft == true)
        {
            transform.position = leftPosition.position;
            leftRing.moveLeft = false;
            rb.velocity = Vector2.zero;
        }

        if (rightRing.moveRight == true)
        {
            currentDirection = rightPosition.position - leftPosition.position;
            currentDirection.Normalize();
            rb.velocity = currentDirection * speed;
        }
        else if (leftRing.moveLeft == true)
        {
            currentDirection = leftPosition.position - rightPosition.position;
            currentDirection.Normalize();
            rb.velocity = currentDirection * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

    }
}
