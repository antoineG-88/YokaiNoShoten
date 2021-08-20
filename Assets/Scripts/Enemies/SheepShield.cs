using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepShield : MonoBehaviour
{
    [HideInInspector]
    public Sheep connectedSheep;
    [HideInInspector]
    public Enemy enemy;
    [HideInInspector]
    public bool isActive;
    private SpriteRenderer sr;
    [HideInInspector]
    public LineRenderer shieldConnection;
    public float distanceToEndLine;

    private Vector3[] connectionPos;
    void Start()
    {
        enemy.isProtected = true;
        sr = GetComponent<SpriteRenderer>();
        shieldConnection = GetComponent<LineRenderer>();
        connectionPos = new Vector3[2];
    }

    Vector2 sheepDirection;
    void Update()
    {
        if(isActive)
        {
            shieldConnection.enabled = true;
            enemy.isProtected = true;
            sheepDirection = connectedSheep.transform.position - transform.position;
            sheepDirection.Normalize();
            connectionPos[0] = (Vector2)transform.position + sheepDirection * distanceToEndLine;
            connectionPos[1] = connectedSheep.transform.position;
            shieldConnection.SetPositions(connectionPos);
        }
        else
        {
            shieldConnection.enabled = false;
        }
    }

    public void Disabling()
    {
        enemy.isProtected = false;
        isActive = false;
        sr.enabled = false;
    }
}
