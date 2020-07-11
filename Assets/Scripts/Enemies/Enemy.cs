﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class Enemy : MonoBehaviour
{
    [Header("General settings")]
    public int maxHealthPoint;
    [Header("Pathfinding settings")]
    public float nextWaypointDistance;
    public int waypointAhead;
    public float pathUpdatingFrequency;
    [Space]
    public bool avoidEnemies;
    public float avoidEffectRadius;
    public float avoidForce;
    public float minimalAvoidForce;
    public float maximalAvoidForce;
    public float avoidDistanceInfluence;

    private int currentHealthPoint;

    protected Rigidbody2D rb;
    protected Seeker seeker;

    protected Path path;
    protected int currentWaypoint;
    protected bool pathEndReached;
    protected Vector2 pathDirection;
    protected Vector2 targetPathfindingPosition;
    protected float timeBeforeNextPathUpdate;

    protected bool provoked;

    protected Animator animator;

    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        currentHealthPoint = maxHealthPoint;
        pathEndReached = false;
        provoked = false;
        targetPathfindingPosition = transform.position;
    }
    protected void Update()
    {
        UpdatePath();
    }

    protected void FixedUpdate()
    {
        AvoidOtherEnemies();
    }

    public void CalculatePath()
    {
        seeker.StartPath(transform.position, targetPathfindingPosition, OnPathComplete);
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void UpdatePath()
    {
        if (timeBeforeNextPathUpdate <= 0)
        {
            timeBeforeNextPathUpdate = pathUpdatingFrequency;

            CalculatePath();

            if (path != null)
            {
                if (currentWaypoint >= path.vectorPath.Count)
                {
                    pathEndReached = true;
                }
                else
                {
                    pathEndReached = false;

                    while (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance && path.vectorPath.Count - waypointAhead > currentWaypoint + 1)
                    {
                        currentWaypoint++;
                    }

                    pathDirection = (path.vectorPath[currentWaypoint + waypointAhead] - transform.position).normalized;
                }
            }
        }

        if (timeBeforeNextPathUpdate > 0)
        {
            timeBeforeNextPathUpdate -= Time.deltaTime;
        }
    }

    private void AvoidOtherEnemies()
    {
        if (avoidEnemies)
        {
            ContactFilter2D enemyFilter = new ContactFilter2D();
            enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            List<Collider2D> closeEnemies = new List<Collider2D>();
            Physics2D.OverlapCircle(transform.position, avoidEffectRadius, enemyFilter, closeEnemies);
            foreach (Collider2D closeEnemy in closeEnemies)
            {
                if (closeEnemy.gameObject != gameObject)
                {
                    Vector2 directedForce = ((closeEnemy.transform.position - transform.position).normalized * avoidForce);
                    if (avoidDistanceInfluence != 0)
                    {
                        directedForce /= avoidDistanceInfluence * Vector2.Distance(transform.position, closeEnemy.transform.position);
                    }

                    if (directedForce.magnitude < minimalAvoidForce)
                    {
                        directedForce = directedForce.normalized * minimalAvoidForce;
                    }
                    else if (directedForce.magnitude > maximalAvoidForce)
                    {
                        directedForce = directedForce.normalized * maximalAvoidForce;
                    }

                    closeEnemy.GetComponent<Enemy>().Propel(directedForce * Time.fixedDeltaTime);
                }
            }
        }
    }
    public abstract void UpdateMovement();

    public void TakeDamage(int damage, Vector2 directedForce)
    {
        currentHealthPoint -= damage;
        Propel(directedForce);
        animator.SetTrigger("Hurt");
        if(currentHealthPoint <= 0)
        {
            Die();
        }
    }

    public void Propel(Vector2 directedForce)
    {
        rb.velocity += directedForce;
    }

    private void Die()
    {
        Destroy(gameObject, 0.5f);
    }
}
