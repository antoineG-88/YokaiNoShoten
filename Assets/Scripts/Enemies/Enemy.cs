using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class Enemy : MonoBehaviour
{
    [Header("General settings")]
    public int maxHealthPoint;
    public List<BodyPart> damagableBodyParts;
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

    [HideInInspector] public bool recentlyHit;
    private int currentHealthPoint;

    protected Rigidbody2D rb;
    protected Seeker seeker;

    protected Path path;
    protected int currentWaypoint;
    protected bool pathEndReached;
    protected Vector2 pathDirection;
    protected List<Vector3> pathPositions;
    protected Vector2 targetPathfindingPosition;
    protected float timeBeforeNextPathUpdate;
    protected float distToPlayer;

    protected bool provoked;
    [HideInInspector] public bool inControl;

    protected Animator animator;
    protected ProtectionHandler protectionHandler;

    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        currentHealthPoint = maxHealthPoint;
        pathEndReached = false;
        provoked = false;
        targetPathfindingPosition = transform.position;
        inControl = true;
        pathPositions = new List<Vector3>();
        protectionHandler = GetComponent<ProtectionHandler>();
        ConnectBodyParts();
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

        UpdateBehavior();
    }

    protected void FixedUpdate()
    {
        recentlyHit = false;
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
                pathPositions.Clear();
                pathPositions.AddRange(path.vectorPath);
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
                    if (directedForce == Vector2.zero)
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
    protected virtual void UpdateBehavior()
    {
        distToPlayer = Vector2.Distance(transform.position, GameData.player.transform.position);
    }

    public void TakeDamage(int damage, Vector2 directedForce, float noControlTime, bool isOtherBodyPart)
    {
        if(!recentlyHit)
        {
            bool isProtected = false;
            if (!isOtherBodyPart && protectionHandler != null)
            {
                isProtected = protectionHandler.IsProtected(directedForce);
            }

            if (!isProtected)
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

            recentlyHit = true;
        }
    }

    public void Propel(Vector2 directedForce)
    {
        if (!float.IsNaN(directedForce.magnitude))
        {
            rb.velocity += directedForce;
        }
    }

    protected bool IsPlayerInSightFrom(Vector2 startPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, (Vector2)GameData.player.transform.position - startPos, 100, LayerMask.GetMask("Player", "Wall"));
        if (hit && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected Vector2 FindNearestSightSpot(float angleInterval, float distance, bool addShorterSpot)
    {
        Vector2 spot = transform.position;
        bool validSpotFound = false;
        float nearestAngle = Vector2.SignedAngle(Vector2.right, transform.position - GameData.player.transform.position);
        nearestAngle /= angleInterval;
        nearestAngle = Mathf.Round(nearestAngle) * angleInterval;
        float angleSeekOffset = 0;
        int iteration = 0;
        bool seekPositive = true;
        float seekAngle = 0;
        Vector2 seekDirection;
        while (angleSeekOffset < 181 && !validSpotFound && angleInterval != 0)
        {
            angleSeekOffset = angleInterval * iteration;
            if (seekPositive)
            {
                seekAngle = nearestAngle + angleSeekOffset;
            }
            else
            {
                seekAngle = nearestAngle - angleSeekOffset;
                iteration++;
            }
            seekPositive = !seekPositive;

            seekDirection = DirectionFromAngle(seekAngle);
            spot = (Vector2)GameData.player.transform.position + seekDirection * distance;
            if(addShorterSpot)
            {
                validSpotFound = true;
                RaycastHit2D hit = Physics2D.Raycast(GameData.player.transform.position, spot - (Vector2)GameData.player.transform.position, distance, LayerMask.GetMask("Wall"));
                if (hit)
                {
                    spot = hit.point;
                }
            }
            else
            {
                if (IsPlayerInSightFrom(spot))
                {
                    validSpotFound = true;
                }
            }
            Debug.DrawRay(spot, Vector2.up * 0.1f, validSpotFound ? Color.green : Color.white, 0.02f);
        }

        return validSpotFound ? spot : (Vector2)transform.position;
    }

    protected Vector2 DirectionFromAngle(float angle)
    {
        Vector2 direction;
        direction.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        direction.y = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.Normalize();
        return direction;
    }

    protected Vector2 GetPathNextPosition(int positionAhead)
    {
        if(pathPositions.Count > 0)
        {
            if (pathPositions.Count > currentWaypoint + waypointAhead + positionAhead)
            {
                return pathPositions[currentWaypoint + waypointAhead + positionAhead];
            }
            else
            {
                return pathPositions[currentWaypoint + waypointAhead];
            }
        }
        else
        {
            return Vector2.zero;
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

    public void ConnectBodyParts()
    {
        foreach(BodyPart bodyPart in damagableBodyParts)
        {
            bodyPart.owner = this;
        }
    }
}
