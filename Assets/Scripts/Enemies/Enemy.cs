using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class Enemy : MonoBehaviour
{
    [Header("General settings")]
    public int maxHealthPoint;
    public bool useShield;
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
    [HideInInspector] public bool inControl;

    protected Animator animator;
    protected ProtectionHandler protectionHandler;

    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if(useShield)
        {

        }
        else
        {
            protectionHandler = GetComponent<ProtectionHandler>();
        }
        seeker = GetComponent<Seeker>();
        currentHealthPoint = maxHealthPoint;
        pathEndReached = false;
        provoked = false;
        targetPathfindingPosition = transform.position;
        inControl = true;
    }
    protected void Update()
    {
        if (timeBeforeNextPathUpdate <= 0)
        {
            timeBeforeNextPathUpdate = pathUpdatingFrequency;
            UpdatePath();
        }

        if (timeBeforeNextPathUpdate > 0)
        {
            timeBeforeNextPathUpdate -= Time.deltaTime;
        }
    }

    protected void FixedUpdate()
    {
        AvoidOtherEnemies();
        UpdateMovement();
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

    protected void UpdatePath()
    {
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
                    Vector2 directedForce = closeEnemy.transform.position - transform.position;
                    if(directedForce == Vector2.zero)
                    {
                        directedForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    }

                    directedForce = directedForce.normalized * avoidForce;
                    if (avoidDistanceInfluence != 0)
                    {
                        directedForce /= avoidDistanceInfluence * Mathf.Clamp(Vector2.Distance(transform.position, closeEnemy.transform.position), 0.01f, 100);
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

    public void TakeDamage(int damage, Vector2 directedForce, float noControlTime)
    {
        bool isProtected = false;
        if (!useShield)
        {
            if(protectionHandler.TestProtection(directedForce.normalized))
            {
                isProtected = true;
            }
        }

        if(!isProtected)
        {
            currentHealthPoint -= damage;
            Propel(directedForce);
            animator.SetTrigger("Hurt");
            StartCoroutine(NoControl(noControlTime));
            if (currentHealthPoint <= 0)
            {
                Die();
            }
        }
    }

    public void Propel(Vector2 directedForce)
    {
        if(!float.IsNaN(directedForce.magnitude))
        {
            rb.velocity += directedForce;
        }
    }

    private void Die()
    {
        Destroy(gameObject, 0.1f);
    }

    public IEnumerator NoControl(float time)
    {
        inControl = false;
        yield return new WaitForSeconds(time);
        inControl = true;
    }
}
