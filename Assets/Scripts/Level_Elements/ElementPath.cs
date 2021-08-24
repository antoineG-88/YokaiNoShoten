using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPath : MonoBehaviour
{
    public Transform[] pathPositions;
    public bool useSpeed;
    public float speed;
    public float timeToCompleteFullLoop;
    public bool isBackAndForth;
    public bool endLoopByTp;
    public bool inactiveResetToStartPos;
    [Range(0.0f, 100.0f)] public float startProgression;
    public Switch connectedSwitch;

    private Vector2 currentDirection;
    private Vector2[] pathVectors;
    private float pathLength;
    Rigidbody2D rb;
    private bool isActive;
    private float currentSpeed;
    private float loopTime;
    private float currentProgression;
    private float progressionSpeed;
    private int currentTargetPositonIndex;
    private int previousTargetPositionIndex;
    private bool isGoingBack;

    void Start()
    {
        isGoingBack = false;
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
        }

        if (useSpeed)
        {
            currentSpeed = speed;
            loopTime = speed * pathLength;
        }
        else
        {
            currentSpeed = pathLength / timeToCompleteFullLoop;
            loopTime = timeToCompleteFullLoop;
        }

        progressionSpeed = currentSpeed / pathLength;
        currentProgression = startProgression / 100;

        if (startProgression != 0 && startProgression != 100)
        {
            transform.position = GetPosProgressionInPath(startProgression / 100);
        }
        else
        {
            currentTargetPositonIndex = 1;
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
        if(connectedSwitch != null)
        {
            isActive = connectedSwitch.IsON();
        }

        if(isActive)
        {
            rb.velocity = currentDirection * currentSpeed;
            if(!isBackAndForth)
            {
                currentProgression += progressionSpeed * Time.fixedDeltaTime;

                if (currentProgression > 1)
                {
                    currentProgression -= 1;
                }
            }
            else
            {
                currentProgression += progressionSpeed * Time.fixedDeltaTime * (isGoingBack ? -1 : 1);

                if(currentProgression < 0)
                {
                    isGoingBack = false;
                }

                if(currentProgression > 1)
                {
                    isGoingBack = true;
                }
            }


            transform.position = GetPosProgressionInPath(currentProgression);
        }
        else
        {
            rb.velocity = Vector2.zero;
            if(inactiveResetToStartPos)
            {
                if (startProgression != 0 && startProgression != 100)
                {
                    transform.position = GetPosProgressionInPath(startProgression / 100);
                }
                else
                {
                    if(startProgression == 0 || (!endLoopByTp && !isBackAndForth))
                    {
                        transform.position = pathPositions[0].position;
                    }
                    else
                    {
                        transform.position = pathPositions[pathPositions.Length - 1].position;
                    }
                }
            }
        }
    }

    private void RefreshDirection()
    {
        currentDirection = pathPositions[currentTargetPositonIndex].position - pathPositions[previousTargetPositionIndex].position;
        currentDirection.Normalize();
    }

    private Vector2 GetPosProgressionInPath(float progression)
    {
        float startDistance = progression * pathLength;
        float distance = 0;
        Vector2 pos = Vector2.zero;


        if (isBackAndForth)
        {
            int i = 0;
            while (distance != startDistance && i != pathPositions.Length)
            {
                if (distance + pathVectors[i].magnitude < startDistance)
                {
                    distance += pathVectors[i].magnitude;
                }
                else
                {
                    pos = Vector2.Lerp(pathPositions[i].position, pathPositions[i + 1 < pathPositions.Length ? i + 1 : 0].position, (startDistance - distance) / pathVectors[i].magnitude);
                    distance = startDistance;

                    if(isGoingBack)
                    {
                        if (currentTargetPositonIndex != i)
                        {
                            previousTargetPositionIndex = currentTargetPositonIndex;
                            currentTargetPositonIndex = i;
                            RefreshDirection();
                        }
                    }
                    else
                    {
                        if (currentTargetPositonIndex != (i + 1 < pathPositions.Length ? i + 1 : i - 1))
                        {
                            previousTargetPositionIndex = currentTargetPositonIndex;
                            currentTargetPositonIndex = i + 1 < pathPositions.Length ? i + 1 : i - 1;
                            if (endLoopByTp && currentTargetPositonIndex == 1)
                            {
                                previousTargetPositionIndex = 0;
                            }
                            RefreshDirection();
                        }
                    }
                }

                i++;
            }
        }
        else
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
                    pos = Vector2.Lerp(pathPositions[i].position, pathPositions[i + 1 < pathPositions.Length ? i + 1 : 0].position, (startDistance - distance) / pathVectors[i].magnitude);
                    distance = startDistance;
                    if(currentTargetPositonIndex != (i + 1 < pathPositions.Length ? i + 1 : endLoopByTp ? 1 : 0))
                    {
                        previousTargetPositionIndex = currentTargetPositonIndex;
                        currentTargetPositonIndex = i + 1 < pathPositions.Length ? i + 1 : endLoopByTp ? 1 : 0;
                        if (endLoopByTp && currentTargetPositonIndex == 1)
                        {
                            previousTargetPositionIndex = 0;
                        }
                        RefreshDirection();
                    }
                }
                i++;
            }
        }

        return pos;
    }

    private void OnDrawGizmos()
    {
        if(pathPositions.Length > 0)
        {
            for (int i = 0; i < pathPositions.Length; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(pathPositions[i].position, pathPositions[i < pathPositions.Length - 1 ? i + 1 : 0].position);
            }
        }
    }
}
