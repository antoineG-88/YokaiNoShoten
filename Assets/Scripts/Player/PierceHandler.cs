using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceHandler : MonoBehaviour
{
    public float pierceTriggerRange;
    public float pierceMoveTime;
    public float positionDistanceBehindEnemy;
    public AnimationCurve pierceMovementCurve;
    public float pierceAttackKnockbackForce;
    public float maxPhasingTime;
    public LayerMask enemyMask;
    public GameObject pierceShadowFx;
    public GameObject pierceMarkFx;
    public float timeBeforeFirstPierce;
    public bool useSingleAttack;
    public float damageDelay;
    public float slowMoDelay;
    public Transform pierceArrowPreview;
    public Transform pierceEndPosPreview;
    public bool triggerSlowMo;

    private ContactFilter2D enemyFilter;
    private Vector2 enemyDirection;
    [HideInInspector] public bool isPiercing;
    [HideInInspector] public bool isPhasing;
    private Rigidbody2D rb;
    private float startPhasingTime;
    [HideInInspector] public bool canPierce;

    private void Start()
    {
        enemyFilter.SetLayerMask(enemyMask);
        enemyFilter.useTriggers = true;
        rb = GetComponent<Rigidbody2D>();
        markedObjects = new List<GameObject>();
        nearestObject = null;
        colliders = new List<Collider2D>();
    }

    private void Update()
    {
        MarkPierce();
        UpdatePhasingTime();

        if (GameData.movementHandler.isGrounded && !GameData.movementHandler.isOnSlope)
        {
            canPierce = true;
        }
    }

    private List<GameObject> markedObjects;
    private GameObject nearestObject;
    List<Collider2D> colliders;
    private void MarkPierce() //Affiche la preview du pierce et Marque les ennemis ou objets touché par le déclenchement de l'attaque et la lance s'il y en a eu
    {
        colliders.Clear();
        Physics2D.OverlapCircle(transform.position, pierceTriggerRange, enemyFilter, colliders);
        if (colliders.Count > 0)
        {
            markedObjects.Clear();
            nearestObject = null;
            float minDist = 500;
            for (int i = 0; i < colliders.Count; i++)
            {
                if (nearestObject == null || Vector2.Distance(colliders[i].transform.position, transform.position) < minDist)
                {
                    nearestObject = colliders[i].gameObject;
                    minDist = Vector2.Distance(colliders[i].transform.position, transform.position);
                }
            }

            Enemy potentialNearestEnemy = nearestObject.GetComponent<Enemy>();
            if ((potentialNearestEnemy != null && !potentialNearestEnemy.isDying) || potentialNearestEnemy == null)
            {
                markedObjects.Add(nearestObject);
                pierceArrowPreview.gameObject.SetActive(true);
                pierceArrowPreview.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, nearestObject.transform.position - transform.position));

                Vector2 objectDirection = nearestObject.transform.position - transform.position;
                objectDirection.Normalize();
                pierceEndPosPreview.gameObject.SetActive(true);
                pierceEndPosPreview.position = (Vector2)nearestObject.transform.position + objectDirection * positionDistanceBehindEnemy;
            }
            else
            {
                markedObjects.Clear();
                nearestObject = null;
                pierceArrowPreview.gameObject.SetActive(false);
                pierceEndPosPreview.gameObject.SetActive(false);
            }

        }
        else
        {
            pierceArrowPreview.gameObject.SetActive(false);
            pierceEndPosPreview.gameObject.SetActive(false);
            nearestObject = null;
        }

        if (Input.GetButtonDown("AButton") && canPierce)
        {
            canPierce = false;
            StopPhasingTime();

            List<Collider2D> colliders = new List<Collider2D>();
            Physics2D.OverlapCircle(transform.position, pierceTriggerRange, enemyFilter, colliders);
            if (colliders.Count > 0)
            {
                float minDist = 500;
                for (int i = 0; i < colliders.Count; i++)
                {
                    if (nearestObject == null || Vector2.Distance(colliders[i].transform.position, transform.position) < minDist)
                    {
                        nearestObject = colliders[i].gameObject;
                        minDist = Vector2.Distance(colliders[i].transform.position, transform.position);
                    }
                }
                if ((nearestObject.GetComponent<Enemy>() != null && !nearestObject.GetComponent<Enemy>().isDying) || nearestObject.GetComponent<Enemy>() == null)
                    markedObjects.Add(nearestObject);

                if (!useSingleAttack)
                {
                    for (int i = 0; i < colliders.Count; i++)
                    {
                        if (nearestObject != colliders[i].gameObject)
                        {
                            if ((colliders[i].GetComponent<Enemy>() != null && !colliders[i].GetComponent<Enemy>().isDying) || colliders[i].GetComponent<Enemy>() == null)
                                markedObjects.Add(colliders[i].gameObject);
                        }
                    }
                }

                StartCoroutine(Pierce(markedObjects));
            }
            else
            {
                //feedback lancé attaque dans le vide
            }
        }
    }


    private IEnumerator Pierce(List<GameObject> markedEnemies)
    {
        GameData.grappleHandler.ReleaseHook();
        rb.velocity = Vector2.zero;
        GameData.movementHandler.canMove = false;
        GameData.movementHandler.isAffectedbyGravity = false;
        isPiercing = true;
        yield return new WaitForSeconds(timeBeforeFirstPierce);

        if(triggerSlowMo)
            StartPhasingTime();

        Vector2 enemyPosition;
        Vector2 startPiercePos;
        Vector2 currentPiercePos;
        Vector2 previousPiercePos;
        float pierceTimeElapsed = 0;
        float currentPierceSpeed;
        Vector2 pierceEndPos;

        for (int i = 0; i < markedEnemies.Count; i++)
        {
            enemyPosition = markedEnemies[i].transform.position;
            enemyDirection = enemyPosition - (Vector2)transform.position;
            enemyDirection.Normalize();
            pierceEndPos = enemyPosition + enemyDirection * positionDistanceBehindEnemy;
            startPiercePos = transform.position;
            currentPiercePos = transform.position;
            previousPiercePos = transform.position;

            Enemy enemy = markedEnemies[i].GetComponent<Enemy>();
            if(damageDelay <= 0)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(1, -enemy.GetComponent<Rigidbody2D>().velocity, 0.5f, false);
                    Instantiate(pierceMarkFx, enemy.transform.position, Quaternion.identity).transform.localScale = new Vector3(1, 1, 1);
                }
            }
            pierceTimeElapsed = 0;
            while (pierceTimeElapsed < pierceMoveTime && GameData.playerManager.inControl && isPiercing)
            {
                pierceTimeElapsed += Time.fixedDeltaTime;
                if(pierceTimeElapsed > damageDelay && damageDelay > 0)
                {
                    if (enemy != null)
                    {
                        enemy.TakeDamage(1, -enemy.GetComponent<Rigidbody2D>().velocity, 0.5f, false);
                        Instantiate(pierceMarkFx, enemy.transform.position, Quaternion.identity).transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                Instantiate(pierceShadowFx, transform.position, Quaternion.identity).transform.localScale = new Vector3(enemyDirection.x > 0 ? 1 : -1, 1, 1);
                currentPiercePos = Vector2.LerpUnclamped(startPiercePos, pierceEndPos, pierceMovementCurve.Evaluate(pierceTimeElapsed / pierceMoveTime));
                currentPierceSpeed = (currentPiercePos - previousPiercePos).magnitude;
                previousPiercePos = currentPiercePos;

                rb.velocity = enemyDirection * currentPierceSpeed * (1 / Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }
        }
        isPiercing = false;
        GameData.movementHandler.canMove = true;
        GameData.movementHandler.isAffectedbyGravity = true;
        canPierce = true;
        GameData.dashHandler.canDash = true;
    }

    private void UpdatePhasingTime()
    {
        if(isPhasing)
        {
            if(startPhasingTime < Time.realtimeSinceStartup - maxPhasingTime)
            {
                StopPhasingTime();
            }
        }
    }

    public void StartPhasingTime()
    {
        startPhasingTime = Time.realtimeSinceStartup;
        if(!isPhasing)
            StartCoroutine(GameData.slowMoManager.SmoothStartSlowMo(1, slowMoDelay));
        isPhasing = true;
    }

    public void StopPhasingTime()
    {
        if(isPhasing)
            StartCoroutine(GameData.slowMoManager.SmoothStopSlowMo(0));
        isPhasing = false;
    }
}
