using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPath : MonoBehaviour
{
    public Transform[] pathPositions;
    public float speed;
    public bool isBackAndForth;
    public bool endLoopByTp;
    [Range(0.0f, 100.0f)] public float startProgression;
    public float pauseTime;
    public Switch connectedSwitch;

    private int currentTargetPositon;
    private int previousTargetPosition;
    private bool isReturning;

    private Vector2 currentDirection;
    private Vector2[] pathVectors;
    private float pathLength;
    Rigidbody2D rb;
    private bool isActive;

    void Start()
    {
        isActive = true;
        rb = GetComponent<Rigidbody2D>();
        pathVectors = new Vector2[pathPositions.Length];
        pathLength = 0;
        for (int i = 0; i < pathVectors.Length; i++)
        {
            if(i < pathVectors.Length - 1)
            {
                pathVectors[i] = pathPositions[i+1].position - pathPositions[i].position;

                if (isBackAndForth || endLoopByTp)
                {
                    pathLength += pathVectors[i].magnitude;
                }
            }
            else
            {
                pathVectors[i] = pathPositions[0].position - pathPositions[i].position;
            }

            if(!isBackAndForth && !endLoopByTp)
            {
                pathLength += pathVectors[i].magnitude;
            }
            else if(isBackAndForth)
            {
                pathLength *= 2;
            }
        }

        if(startProgression != 0 && startProgression != 100 && !isBackAndForth)
        {
            float startDistance = startProgression / 100 * pathLength;
            float distance = 0;
            if (!isBackAndForth)
            {
                int i = 0;
                while (distance != startDistance && i < pathPositions.Length)
                {
                    if (distance + pathVectors[i].magnitude < startDistance)
                    {
                        distance += pathVectors[i].magnitude;
                    }
                    else
                    {
                        transform.position = Vector2.Lerp(pathPositions[i].position, pathPositions[i + 1 < pathPositions.Length ? i + 1 : 0].position, (startDistance - distance) / pathVectors[i].magnitude);
                        currentDirection = pathVectors[i].normalized;
                        currentTargetPositon = i + 1 < pathPositions.Length ? i + 1 : 0;
                        distance = startDistance;
                    }
                    i++;
                }
            }
        }
        else
        {
            currentTargetPositon = 1;
            transform.position = pathPositions[0].position;
            currentDirection = pathPositions[1].position - pathPositions[0].position;
            currentDirection.Normalize();
        }
    }


    void FixedUpdate()
    {
        UpdateDirection();
    }


    private void UpdateDirection()
    {
        if (Vector2.Distance(transform.position, pathPositions[currentTargetPositon].position) < Time.fixedDeltaTime * speed *3)
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
                    if(endLoopByTp)
                    {
                        currentTargetPositon = 1;
                        previousTargetPosition = 0;
                        transform.position = pathPositions[0].position;
                    }
                    else
                    {
                        currentTargetPositon = 0;
                    }
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

        if(connectedSwitch != null)
        {
            isActive = connectedSwitch.isOn;
        }

        if(isActive)
        {
            rb.velocity = currentDirection * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pathPositions.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pathPositions[i].position, pathPositions[i < pathPositions.Length - 1 ? i + 1 : 0].position);
        }
    }
}
