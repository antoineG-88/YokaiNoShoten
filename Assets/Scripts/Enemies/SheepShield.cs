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

    private Vector3[] connectionPos;
    void Start()
    {
        enemy.isProtected = true;
        sr = GetComponent<SpriteRenderer>();
        shieldConnection = GetComponent<LineRenderer>();
        connectionPos = new Vector3[2];
    }

    void Update()
    {
        if(isActive)
        {
            shieldConnection.enabled = true;
            enemy.isProtected = true;
            connectionPos[0] = transform.position;
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
