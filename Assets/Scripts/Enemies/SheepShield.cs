using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepShield : MonoBehaviour
{
    public LineRenderer sheepConnection;
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
    public float distanceToStartLine;
    public float distanceToStartSheepLine;
    public float distanceToEndSheepLine;

    private Vector3[] connectionPos;
    private Vector3[] sheepConnectionPos;

    void Start()
    {
        enemy.isProtected = true;
        sr = GetComponent<SpriteRenderer>();
        shieldConnection = GetComponent<LineRenderer>();
        connectionPos = new Vector3[2];
        sheepConnectionPos = new Vector3[2];
    }

    Vector2 sheepDirection;
    void Update()
    {
        if(isActive)
        {
            enemy.isProtected = true;
            shieldConnection.enabled = true;
            sheepDirection = connectedSheep.transform.position - transform.position;
            sheepDirection.Normalize();
            connectionPos[0] = (Vector2)transform.position + sheepDirection * distanceToStartLine;
            connectionPos[1] = (Vector2)transform.position + sheepDirection * distanceToEndLine;
            shieldConnection.SetPositions(connectionPos);

            sheepConnectionPos[0] = (Vector2)connectedSheep.transform.position - sheepDirection * distanceToStartSheepLine;
            sheepConnectionPos[1] = (Vector2)connectedSheep.transform.position - sheepDirection * distanceToEndSheepLine;
            sheepConnection.SetPositions(sheepConnectionPos);
        }
    }

    public void Disabling()
    {
        enemy.isProtected = false;
        shieldConnection.enabled = false;
        sheepConnection.enabled = false;
        isActive = false;
        sr.enabled = false;
    }
}
