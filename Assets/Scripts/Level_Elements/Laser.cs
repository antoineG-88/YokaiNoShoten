using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float maxLaserRange;
    public float beamWidth;
    public float beamWallWidth;
    public float knockbackDistance;
    public bool oddIndexActive;
    public List<float> activationsSequence;

    public Switch connectedSwitch;
    [Header("Technical settings")]
    public GameObject beamStartPrefab;
    public GameObject beamPartPrefab;
    public GameObject beamImpactPrefab;
    public float spaceBetweenBeamFx;
    public float beamStartOffset;
    public float beamDisplayStartOffset;
    public BoxCollider2D boxCollider;
    public Transform beamParent;

    private Vector2 currentDirection;
    private float beamLength;
    private int beamFxNumber;
    private List<GameObject> beamFxs = new List<GameObject>();
    private GameObject beamEnd = null;
    private RaycastHit2D hit;
    private RaycastHit2D playerHit;
    private RaycastHit2D enemyHit;

    private bool isActive;
    private bool isBeamActive;
    private float elapsedSequenceTime;
    private int currentSequenceIndex;

    void Start()
    {
        isActive = true;
        if(oddIndexActive)
        {
            isBeamActive = false;
        }
        else
        {
            isBeamActive = true;
        }


    }

    void Update()
    {
        currentDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z);
        if(connectedSwitch != null)
        {
            isActive = connectedSwitch.IsON();
        }

        if(activationsSequence.Count > 0)
        {
            UpdateSequence();
        }
    }

    private void FixedUpdate()
    {
        UpdateLaserBeam();
    }

    private void UpdateLaserBeam()
    {
        if(isBeamActive && isActive)
        {
            hit = Physics2D.Raycast((Vector2)transform.position + currentDirection * beamStartOffset, currentDirection, maxLaserRange, LayerMask.GetMask("Wall","DashWall"));

            playerHit = Physics2D.CircleCast((Vector2)transform.position + currentDirection * beamStartOffset + currentDirection * beamWidth / 2, beamWidth, currentDirection, maxLaserRange, LayerMask.GetMask("Player", "Wall", "DashWall"));
            if (playerHit && playerHit.collider.CompareTag("Player"))
            {
                Vector2 knockbackDirection;
                if(Vector2.SignedAngle(currentDirection, GameData.player.transform.position - transform.position) > 0)
                {
                    knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z + 90);
                }
                else
                {
                    knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z - 90);
                }
                GameData.dashHandler.isDashing = false;
                GameData.playerManager.TakeDamage(1, knockbackDirection * knockbackDistance);
            }

            /*
            enemyHit = Physics2D.CircleCast((Vector2)transform.position + currentDirection * beamStartOffset + currentDirection * beamWidth / 2, beamWidth, currentDirection, maxLaserRange, LayerMask.GetMask("Enemy", "Wall", "NoInteraction"));
            if (enemyHit && enemyHit.collider.CompareTag("Enemy"))
            {
                Enemy hitEnemy = enemyHit.collider.GetComponent<Enemy>();
                if(hitEnemy.currentSheepShield != null)
                {
                    Vector2 knockbackDirection;
                    if (Vector2.SignedAngle(currentDirection, hitEnemy.transform.position - transform.position) > 0)
                    {
                        knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z + 90);
                    }
                    else
                    {
                        knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z - 90);
                    }
                    hitEnemy.Propel(knockbackDirection * knockbackDistance * 2);
                    StartCoroutine(hitEnemy.NoControl(0.3f));
                    hitEnemy.currentSheepShield.Disabling();
                }
            }*/


            beamLength = hit ? Vector2.Distance(transform.position, hit.point) - beamDisplayStartOffset : maxLaserRange;
            beamFxNumber = Mathf.CeilToInt(beamLength / spaceBetweenBeamFx);


            boxCollider.enabled = true;
            boxCollider.offset = new Vector2(beamStartOffset + (beamLength / 2), 0);
            boxCollider.size = new Vector2(beamLength, beamWallWidth);

            beamParent.rotation = Quaternion.identity;

            if (beamFxs.Count < beamFxNumber)
            {
                for (int i = beamFxs.Count; i < beamFxNumber; i++)
                {
                    beamFxs.Add(Instantiate(i == 0 ? beamStartPrefab : beamPartPrefab, (Vector2)transform.position + Vector2.right * ((spaceBetweenBeamFx * i) + beamDisplayStartOffset), Quaternion.identity, beamParent));
                }
            }
            else if (beamFxs.Count > beamFxNumber)
            {
                for (int i = beamFxs.Count - 1; i > beamFxNumber - 1; i--)
                {
                    if(i >= 0)
                    {
                        Destroy(beamFxs[i]);
                        beamFxs.RemoveAt(i);
                    }
                }
            }

            beamParent.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, currentDirection));

            if (beamEnd == null)
            {
                beamEnd = Instantiate(beamImpactPrefab, transform.position, Quaternion.identity);
            }
            beamEnd.transform.position = hit ? hit.point : (Vector2)transform.position + currentDirection * beamLength;
        }
        else
        {
            for (int i = beamFxs.Count - 1; i >= 0; i--)
            {
                Destroy(beamFxs[i]);
                beamFxs.RemoveAt(i);
            }

            if(beamEnd != null)
            {
                Destroy(beamEnd);
                beamEnd = null;
            }
            boxCollider.enabled = false;
        }
    }

    private void UpdateSequence()
    {
        if(isActive)
        {
            if(elapsedSequenceTime > activationsSequence[currentSequenceIndex])
            {
                elapsedSequenceTime = 0;
                currentSequenceIndex++;
                if(currentSequenceIndex >= activationsSequence.Count)
                {
                    currentSequenceIndex = 0;
                }

                if(oddIndexActive ? currentSequenceIndex % 2 != 0 : currentSequenceIndex % 2 == 0)
                {
                    isBeamActive = true;
                }
                else
                {
                    isBeamActive = false;
                }
            }

            elapsedSequenceTime += Time.deltaTime;
        }
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected()
    {
        hit = Physics2D.Raycast(transform.position, GetDirectionFromAngle(transform.rotation.eulerAngles.z), maxLaserRange, LayerMask.GetMask("Wall"), LayerMask.GetMask("DashWall"));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, hit ? hit.point : (Vector2)transform.position + GetDirectionFromAngle(transform.rotation.eulerAngles.z) * maxLaserRange);
    }
}
