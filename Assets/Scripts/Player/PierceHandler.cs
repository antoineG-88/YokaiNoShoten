using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceHandler : MonoBehaviour
{
    [Header("Pierce settings")]
    public float pierceRange;
    public float pierceMoveTime;
    public float positionDistanceBehindEnemy;
    public AnimationCurve pierceMovementCurve;
    public float pierceAttackKnockbackForce;
    public float maxPhasingTime;
    public LayerMask enemyMask;
    public GameObject pierceShadowFx;
    public GameObject pierceMarkFx;
    public float timeBeforeFirstPierce;
    public float damageDelay;
    public float slowMoDelay;
    public Transform pierceArrowPreview;
    public Transform pierceEndPosPreview;
    public bool triggerSlowMo;
    [Header("Pierce Aim settings")]
    public bool useAutoAim;
    public float pierceAimAssistAngle;
    public float pierceAimAssistRaycastNumber;
    [Header("Combo settings")]
    public bool useCombo;
    public float comboPierceTime;
    public float comboTimeTreshold;
    public float comboPierceRange;
    public float comboPierceAimAssistAngle;
    public float comboPierceAimAssistRaycastNumber;
    [Range(-0.2f, 0.2f)] public float comboPierceTresholdOffset;
    public Transform comboPierceTimingHelper;

    private ContactFilter2D enemyFilter;
    private Vector2 enemyDirection;
    [HideInInspector] public bool isPiercing;
    [HideInInspector] public bool isPhasing;
    private Rigidbody2D rb;
    private float startPhasingTime;
    [HideInInspector] public bool canPierce;
    private bool isAimingComboPierce;
    private bool isAimingPierce;
    private int currentComboPierceStep;
    private float realTimeComboPierceElapsed;
    private float cpAimAssistSubAngle; // cp = Combo Pierce
    private float cpAimAssistFirstAngle;
    private float aimAssistSubAngle;
    private float aimAssistFirstAngle;

    private void Start()
    {
        enemyFilter.SetLayerMask(enemyMask);
        enemyFilter.useTriggers = true;
        rb = GetComponent<Rigidbody2D>();
        nearestObject = null;
        colliders = new List<Collider2D>();

        cpAimAssistSubAngle = comboPierceAimAssistAngle / (comboPierceAimAssistRaycastNumber - 1);
        cpAimAssistFirstAngle = -comboPierceAimAssistAngle / 2;

        aimAssistSubAngle = pierceAimAssistAngle / (pierceAimAssistRaycastNumber - 1);
        aimAssistFirstAngle = -pierceAimAssistAngle / 2;
    }

    private void Update()
    {
        if(useAutoAim)
        {
            CheckFirstPierce();
        }
        else
        {
            UpdatePierceAim();
        }
        UpdatePhasingTime();

        if (GameData.movementHandler.isGrounded && !GameData.movementHandler.isOnSlope)
        {
            canPierce = true;
        }
    }

    private GameObject nearestObject;
    List<Collider2D> colliders;
    private void CheckFirstPierce() //Affiche la preview du pierce et lance l'attaque sur un ennemi à proximité
    {
        colliders.Clear();
        Physics2D.OverlapCircle(transform.position, pierceRange, enemyFilter, colliders);
        if (!isPhasing && colliders.Count > 0 && canPierce)
        {
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
                pierceArrowPreview.gameObject.SetActive(true);
                pierceArrowPreview.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, nearestObject.transform.position - transform.position));

                Vector2 objectDirection = nearestObject.transform.position - transform.position;
                objectDirection.Normalize();
                pierceEndPosPreview.gameObject.SetActive(true);
                pierceEndPosPreview.position = (Vector2)nearestObject.transform.position + objectDirection * positionDistanceBehindEnemy;
            }
            else
            {
                nearestObject = null;
                pierceArrowPreview.gameObject.SetActive(false);
                pierceEndPosPreview.gameObject.SetActive(false);
                comboPierceTimingHelper.gameObject.SetActive(false);
            }

        }
        else
        {
            if(currentComboPierceStep == 0)
            {
                pierceArrowPreview.gameObject.SetActive(false);
                pierceEndPosPreview.gameObject.SetActive(false);
                comboPierceTimingHelper.gameObject.SetActive(false);
            }
            nearestObject = null;
        }

        if ((Input.GetButtonDown("AButton") || Input.GetButtonDown("XButton")) && canPierce && !isPhasing)
        {
            canPierce = false;
            StopPhasingTime();

            List<Collider2D> colliders = new List<Collider2D>();
            Physics2D.OverlapCircle(transform.position, pierceRange, enemyFilter, colliders);
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
                    StartCoroutine(Pierce(nearestObject));
            }
            else
            {
                //feedback lancé attaque dans le vide
            }
        }
    }

    private void UpdatePierceAim() //Affiche la visée du pierce et sélectionne un ennemi visée et attque si appuie dur l'attaque
    {
        Vector2 aimDirection = new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV"));
        if (aimDirection.magnitude > 0.1f && canPierce)
        {
            isAimingPierce = true;
        }
        else
        {
            isAimingPierce = false;
        }
        aimDirection.Normalize();

        if (isAimingPierce)
        {
            pierceArrowPreview.gameObject.SetActive(true);
            pierceArrowPreview.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.up, aimDirection));
        }
        else
        {
            pierceArrowPreview.gameObject.SetActive(false);
        }

        GameObject selectedEnemy = null;

        RaycastHit2D hit;
        float minAngleFound = pierceAimAssistAngle;
        for (int i = 0; i < pierceAimAssistRaycastNumber; i++)
        {
            float relativeAngle = aimAssistFirstAngle + aimAssistSubAngle * i;
            float angledDirection = (Vector2.SignedAngle(Vector2.right, aimDirection)) + relativeAngle;
            Vector2 direction = new Vector2(Mathf.Cos((angledDirection) * Mathf.PI / 180), Mathf.Sin((angledDirection) * Mathf.PI / 180));
            Vector2 raycastOrigin = transform.position;

            Debug.DrawRay(raycastOrigin, direction * pierceRange, Color.cyan);

            hit = Physics2D.Raycast(raycastOrigin, direction, pierceRange, LayerMask.GetMask("Wall", "Enemy"));
            if (hit)
            {
                if ((LayerMask.LayerToName(hit.collider.gameObject.layer) != "Wall") && selectedEnemy != hit.collider.gameObject && Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y)) < minAngleFound)
                {
                    selectedEnemy = hit.collider.gameObject;
                    minAngleFound = Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y));
                }
            }
        }

        if (selectedEnemy != null)
        {
            pierceEndPosPreview.gameObject.SetActive(true);
            Vector2 selectedEnemyDirection = selectedEnemy.transform.position - transform.position;
            selectedEnemyDirection.Normalize();

            pierceEndPosPreview.position = (Vector2)selectedEnemy.transform.position + selectedEnemyDirection * positionDistanceBehindEnemy;


        }
        else
        {
            pierceEndPosPreview.gameObject.SetActive(false);
            comboPierceTimingHelper.gameObject.SetActive(false);
        }

        if (Input.GetButtonDown("AButton") || Input.GetButtonDown("XButton"))
        {
            if(selectedEnemy != null)
            {
                StopPhasingTime();
                StartCoroutine(Pierce(selectedEnemy));
            }
            canPierce = false;
        }
    }

    private IEnumerator Pierce(GameObject markedEnemy)
    {
        currentComboPierceStep = 0;
        GameData.grappleHandler.ReleaseHook();
        rb.velocity = Vector2.zero;
        GameData.movementHandler.canMove = false;
        GameData.movementHandler.isAffectedbyGravity = false;
        isPiercing = true;
        yield return new WaitForSeconds(timeBeforeFirstPierce);

        if(triggerSlowMo)
            StartPhasingTime();

        Vector2 startPiercePos;
        Vector2 currentPiercePos;
        Vector2 previousPiercePos;
        float pierceTimeElapsed;
        float currentPierceSpeed;
        Vector2 pierceEndPos;

        enemyDirection = markedEnemy.transform.position - transform.position;
        enemyDirection.Normalize();
        pierceEndPos = (Vector2)markedEnemy.transform.position + enemyDirection * positionDistanceBehindEnemy;
        startPiercePos = transform.position;
        currentPiercePos = transform.position;
        previousPiercePos = transform.position;

        Enemy enemy = markedEnemy.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.DisableColliderFor(2f);
        }

        if (damageDelay <= 0)
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
            if (pierceTimeElapsed > damageDelay && damageDelay > 0)
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

        isPiercing = false;
        GameData.movementHandler.canMove = true;
        GameData.movementHandler.isAffectedbyGravity = true;
        if(!useCombo)
            canPierce = true;
        GameData.dashHandler.canDash = true;
    }

    private void UpdatePhasingTime()
    {
        if(isPhasing)
        {
            if(useCombo)
            {
                if (!isPiercing && currentComboPierceStep == 0)
                {
                    currentComboPierceStep = 1;
                    realTimeComboPierceElapsed = 0;
                    //start sound for combo timing
                }

                if (currentComboPierceStep == 1)
                {
                    Vector2 comboPierceAimDirection = new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV"));
                    if (comboPierceAimDirection.magnitude > 0.1f)
                    {
                        isAimingComboPierce = true;
                    }
                    else
                    {
                        isAimingComboPierce = false;
                    }

                    if (isAimingComboPierce)
                    {
                        pierceArrowPreview.gameObject.SetActive(true);
                        comboPierceAimDirection.Normalize();
                        pierceArrowPreview.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.up, comboPierceAimDirection));


                        GameObject selectedEnemy = null;

                        RaycastHit2D hit;
                        float minAngleFound = comboPierceAimAssistAngle;
                        for (int i = 0; i < comboPierceAimAssistRaycastNumber; i++)
                        {
                            float relativeAngle = cpAimAssistFirstAngle + cpAimAssistSubAngle * i;
                            float angledDirection = (Vector2.SignedAngle(Vector2.right, comboPierceAimDirection)) + relativeAngle;
                            Vector2 direction = new Vector2(Mathf.Cos((angledDirection) * Mathf.PI / 180), Mathf.Sin((angledDirection) * Mathf.PI / 180));
                            Vector2 raycastOrigin = transform.position;

                            Debug.DrawRay(raycastOrigin, direction * comboPierceRange, Color.cyan);

                            hit = Physics2D.Raycast(raycastOrigin, direction, comboPierceRange, LayerMask.GetMask("Wall", "Enemy"));
                            if (hit)
                            {
                                if ((LayerMask.LayerToName(hit.collider.gameObject.layer) != "Wall") && selectedEnemy != hit.collider.gameObject && Vector2.Angle(direction, new Vector2(comboPierceAimDirection.x, comboPierceAimDirection.y)) < minAngleFound)
                                {
                                    selectedEnemy = hit.collider.gameObject;
                                    minAngleFound = Vector2.Angle(direction, new Vector2(comboPierceAimDirection.x, comboPierceAimDirection.y));
                                }
                            }
                        }

                        if (selectedEnemy != null)
                        {
                            pierceEndPosPreview.gameObject.SetActive(true);
                            Vector2 selectedEnemyDirection = selectedEnemy.transform.position - transform.position;
                            selectedEnemyDirection.Normalize();

                            comboPierceTimingHelper.gameObject.SetActive(true);
                            comboPierceTimingHelper.localScale = Vector3.one * ((comboPierceTime - realTimeComboPierceElapsed) / comboPierceTime);
                            comboPierceTimingHelper.position = selectedEnemy.transform.position;

                            pierceEndPosPreview.position = (Vector2)selectedEnemy.transform.position + selectedEnemyDirection * positionDistanceBehindEnemy;


                            if (Input.GetButtonDown("AButton") || Input.GetButtonDown("XButton"))
                            {
                                if (realTimeComboPierceElapsed >= comboPierceTime + comboPierceTresholdOffset - (comboTimeTreshold / 2)
                                && realTimeComboPierceElapsed <= comboPierceTime + comboPierceTresholdOffset + (comboTimeTreshold / 2))
                                {
                                    canPierce = false;
                                    StopPhasingTime();
                                    StartCoroutine(Pierce(selectedEnemy));
                                }
                                else
                                {
                                    StopPhasingTime();
                                    comboPierceTimingHelper.gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            pierceEndPosPreview.gameObject.SetActive(false);
                            comboPierceTimingHelper.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        pierceArrowPreview.gameObject.SetActive(false);
                        pierceEndPosPreview.gameObject.SetActive(false);
                        comboPierceTimingHelper.gameObject.SetActive(false);
                    }

                    realTimeComboPierceElapsed += Time.deltaTime * 1 / Time.timeScale;
                    if (realTimeComboPierceElapsed > comboPierceTime + (comboTimeTreshold / 2) + comboPierceTresholdOffset)
                    {
                        currentComboPierceStep = 2;
                    }
                }
                else
                {
                    pierceArrowPreview.gameObject.SetActive(false);
                    pierceEndPosPreview.gameObject.SetActive(false);
                    comboPierceTimingHelper.gameObject.SetActive(false);
                }
            }

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
        currentComboPierceStep = 0;
    }
}
