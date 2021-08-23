using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class Enemy : Piercable
{
    [Header("General settings")]
    public int maxHealthPoint;
    public float movementZoneRadius;
    public float provocationRange;
    public GameObject prefabObject;
    public AnimationClip deathAnimClip;
    public List<SpriteRenderer> enemySprites;
    public float dissolveTime;
    public float apparitionTime;
    public bool doGoFantomWhenProtected;
    public int enemyFantomLayerNumber;
    [Header("Sounds")]
    public Sound deathSound;
    public Sound provokedSound;

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
    protected Vector2 playerDirection;
    [HideInInspector]
    public bool isProtected;

    public bool provoked;
    protected Vector2 initialPos;
    [HideInInspector] public bool inControl;
    [HideInInspector] public bool isDying;
    [HideInInspector] public SheepShield currentSheepShield;
    protected Animator animator;
    [HideInInspector] public Material material;
    [HideInInspector] public AudioSource source;
    private bool provokeFlag;
    private int startLayer;

    public ParticleSystem deathParticle;
    float shapeAngle;

    [HideInInspector] public int zoneIndex;

    protected void Start()
    {
        doNotReableCollider = false;
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        source = GetComponent<AudioSource>();
        currentHealthPoint = maxHealthPoint;
        pathEndReached = false;
        provoked = false;
        provokeFlag = true;
        initialPos = transform.position;
        targetPathfindingPosition = transform.position;
        inControl = true;
        pathPositions = new List<Vector3>();
        isProtected = false;
        if(deathParticle != null)
        {
            ParticleSystem.ShapeModule shape = deathParticle.shape;
            //shape.rotation = new Vector3(90, shapeAngle, 0);
        }
        material = Instantiate (enemySprites[0].sharedMaterial);
        for (int i = 0; i < enemySprites.Count; i++)
        {
            enemySprites[i].sharedMaterial = material;
        }

        startLayer = gameObject.layer;
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
        source.pitch = Time.timeScale;
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
                    Vector2 directedForce = transform.position- closeEnemy.transform.position;
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

                    Propel(directedForce);
                    //closeEnemy.GetComponent<Enemy>().Propel(directedForce * Time.fixedDeltaTime);
                }
            }
        }
    }
    public abstract void UpdateMovement();

    Collider2D wallCollider;
    protected virtual void UpdateBehavior()
    {
        distToPlayer = Vector2.Distance(transform.position, GameData.player.transform.position);
        playerDirection = GameData.player.transform.position - transform.position;
        playerDirection.Normalize();

        if(Vector2.Distance(transform.position, initialPos) > movementZoneRadius)
        {
            provoked = false;
        }
        else if (distToPlayer < provocationRange)
        {
            provoked = true;
        }

        wallCollider = Physics2D.OverlapBox(transform.position, Vector2.one * 0.02f, 0, LayerMask.GetMask("Wall"));

        if(wallCollider != null)
        {
            Die();
        }

        if(provokeFlag && provoked)
        {
            provokeFlag = false;
            if(provokedSound.clip != null)
                source.PlayOneShot(provokedSound.clip, provokedSound.volumeScale);
        }
        if (!provokeFlag && !provoked)
        {
            provokeFlag = true;
        }

        if(doGoFantomWhenProtected)
        {
            if (isProtected)
            {
                gameObject.layer = enemyFantomLayerNumber;
            }
            else
            {
                gameObject.layer = startLayer;
            }
        }
    }

    public void TakeDamage(int damage, float noControlTime)
    {
        if(!recentlyHit)
        {
            bool isProtected = false;

            if (!isProtected)
            {
                currentHealthPoint -= damage;
                if(animator != null)
                    animator.SetTrigger("Hurt");
                StartCoroutine(NoControl(noControlTime));
                if (currentHealthPoint <= 0)
                {
                    StartCoroutine(Die());
                }
                DamageEffect();
            }

            recentlyHit = true;
        }
    }

    public abstract void DamageEffect();

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

    protected bool IsLineOfViewClearBetween(Vector2 startPos, Vector2 endPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, endPos - startPos, Vector2.Distance(startPos, endPos), LayerMask.GetMask("Wall"));
        if(hit)
        {
            return false;
        }
        else
        {
            return true;
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

    private IEnumerator Die()
    {
        if (animator != null)
            animator.SetBool("Dead",true);
        isDying = true;
        gameObject.layer = enemyFantomLayerNumber;
        doNotReableCollider = true;
        shapeAngle = Vector2.SignedAngle(Vector2.right, GameData.pierceHandler.piercableDirection);
        if (deathParticle != null)
        {
            ParticleSystem.ShapeModule shape = deathParticle.shape;
            //shape.rotation = new Vector3(90,-shapeAngle, 0);
            deathParticle.Play();
        }
        material.SetFloat("_deadOrAlive", 0);
        OnDie();

        if (deathSound.clip != null)
            source.PlayOneShot(deathSound.clip, deathSound.volumeScale);

        yield return new WaitForSeconds(deathAnimClip != null ? deathAnimClip.length : 0.2f);
        if (deathParticle != null)
            deathParticle.Stop();
        float timer = 0;
        while(timer < dissolveTime)
        {
            timer += Time.deltaTime;
            material.SetFloat("_dissolve", 1 - (timer / dissolveTime));
            yield return new WaitForEndOfFrame();
        }
        material.SetFloat("_dissolve", 0);
        Destroy(gameObject);
    }
    protected virtual void OnDie()
    {
        
    }

    public IEnumerator NoControl(float time)
    {
        inControl = false;
        yield return new WaitForSeconds(time);
        inControl = true;
    }

    public IEnumerator Activate()
    {
        prefabObject.SetActive(true);
        OnActivate();
        inControl = false;
        float timer = 0;
        while (timer < apparitionTime)
        {
            timer += Time.deltaTime;
            material.SetFloat("_dissolve", timer / apparitionTime);
            yield return new WaitForEndOfFrame();
        }
        material.SetFloat("_dissolve", 1);
        inControl = true;
    }

    protected virtual void OnActivate()
    {
        
    }

    public void Deactivate()
    {
        prefabObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, movementZoneRadius);
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        if (!isProtected)
        {
            TakeDamage(damage, 0.5f);
        }
        return false;
    }
}
