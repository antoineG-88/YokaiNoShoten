﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPath : MonoBehaviour
{
    public Transform[] pathPositions;
    public float speed;
    public bool isBackAndForth;
    public float pauseTime;

    private int currentTargetPositon;
    private int previousTargetPosition;
    private bool isReturning;

    private Vector2 currentDirection;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTargetPositon = 1;
        transform.position = pathPositions[0].position;
        currentDirection = pathPositions[1].position - pathPositions[0].position;
        currentDirection.Normalize();
    }


    void FixedUpdate()
    {
        UpdateDirection();
    }


    private void UpdateDirection()
    {

        if (Vector2.Distance(transform.position, pathPositions[currentTargetPositon].position) < 0.01f * speed)
        {
            previousTargetPosition = currentTargetPositon;
            transform.position = pathPositions[currentTargetPositon].position;
            
            if(isReturning == false)
            {
                currentTargetPositon += 1;
            }
            else 
            {
                currentTargetPositon -= 1;
            }

            if (currentTargetPositon == pathPositions.Length)
            {
                if(isBackAndForth == false)
                {
                    currentTargetPositon = 0;
                }
                else
                {
                    currentTargetPositon -= 2;
                    isReturning = true;
                }

            }

            if (currentTargetPositon == -1 && isBackAndForth == true)
            {
                currentTargetPositon += 2;
                isReturning = false;
            }

            currentDirection = pathPositions[currentTargetPositon].position - pathPositions[previousTargetPosition].position;
            currentDirection.Normalize();


        }
        rb.velocity = currentDirection * speed;
    }
}
