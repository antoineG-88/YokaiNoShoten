using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelProtectionLink : MonoBehaviour
{
    public LineRenderer enemyConnection;
    [HideInInspector]
    public Sentinel connectedSentinel;
    [HideInInspector]
    public Enemy enemy;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public LineRenderer sentinelConnection;
    public float distanceToEndLine;
    public float distanceToStartLine;
    public float distanceToStartSentinelLine;
    public float distanceToEndSentinelLine;

    private Vector3[] connectionPos;
    private Vector3[] sentinelConnectionPos;
    private Vector2 sentinelDirection;

    void Start()
    {
        sentinelConnection = GetComponent<LineRenderer>();
        connectionPos = new Vector3[2];
        sentinelConnectionPos = new Vector3[2];
    }
    void Update()
    {
        if (isActive)
        {
            sentinelConnection.enabled = true;
            sentinelDirection = connectedSentinel.transform.position - transform.position;
            sentinelDirection.Normalize();
            connectionPos[0] = (Vector2)transform.position + sentinelDirection * distanceToStartLine;
            connectionPos[1] = (Vector2)transform.position + sentinelDirection * distanceToEndLine;
            sentinelConnection.SetPositions(connectionPos);

            sentinelConnectionPos[0] = (Vector2)connectedSentinel.transform.position - sentinelDirection * distanceToStartSentinelLine;
            sentinelConnectionPos[1] = (Vector2)connectedSentinel.transform.position - sentinelDirection * distanceToEndSentinelLine;
            enemyConnection.SetPositions(sentinelConnectionPos);
        }
    }

    public void Disabling()
    {
        sentinelConnection.enabled = false;
        enemyConnection.enabled = false;
        isActive = false;
    }
}

