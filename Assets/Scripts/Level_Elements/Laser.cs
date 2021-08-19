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
    public float beamWarningTime;
    public float beamChangeStateSpeed;
    public Switch connectedSwitch;
    [Header("Technical settings")]
    public bool usePixelBeam;
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
    private LineRenderer beamLine;
    private float beamState;
    private float beamActivationState;
    private Material beamMaterial;

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
        beamLine = GetComponent<LineRenderer>();
        beamMaterial = Instantiate(beamLine.sharedMaterial);
        beamLine.sharedMaterial = beamMaterial;
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
        hit = Physics2D.Raycast((Vector2)transform.position + currentDirection * beamStartOffset, currentDirection, maxLaserRange, LayerMask.GetMask("Wall"));
        beamLength = hit ? Vector2.Distance(transform.position, hit.point) - beamDisplayStartOffset : maxLaserRange;
        beamLine.SetPosition(0, (Vector2)transform.position + currentDirection * beamDisplayStartOffset);
        beamLine.SetPosition(1, hit ? hit.point : (Vector2)transform.position + currentDirection * beamLength);

        if (isBeamActive && isActive)
        {
            playerHit = Physics2D.CircleCast((Vector2)transform.position + currentDirection * beamStartOffset + currentDirection * beamWidth / 2, beamWidth, currentDirection, maxLaserRange, LayerMask.GetMask("Player", "Wall"));
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



            if(usePixelBeam)
            {
                beamFxNumber = Mathf.CeilToInt(beamLength / spaceBetweenBeamFx);
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
                        if (i >= 0)
                        {
                            Destroy(beamFxs[i]);
                            beamFxs.RemoveAt(i);
                        }
                    }
                }

                if (beamEnd == null)
                {
                    beamEnd = Instantiate(beamImpactPrefab, transform.position, Quaternion.identity);
                }
                beamEnd.transform.position = hit ? hit.point : (Vector2)transform.position + currentDirection * beamLength;
            }
            else
            {

                beamState += beamChangeStateSpeed * Time.fixedDeltaTime;
                beamState = Mathf.Clamp(beamState, 0f, 1f);
                beamMaterial.SetFloat("_previsOrAttack", beamState);

                /*beamActivationState += beamChangeStateSpeed + Time.fixedDeltaTime;
                beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                beamMaterial.SetFloat("_laserSwitch", beamActivationState);*/
                //beamMaterial.SetFloat("_laserSwitch", 1);
            }


            boxCollider.enabled = true;
            boxCollider.offset = new Vector2(beamStartOffset + (beamLength / 2), 0);
            boxCollider.size = new Vector2(beamLength, beamWallWidth);


            beamParent.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, currentDirection));



        }
        else
        {
            if(usePixelBeam)
            {
                for (int i = beamFxs.Count - 1; i >= 0; i--)
                {
                    Destroy(beamFxs[i]);
                    beamFxs.RemoveAt(i);
                }

                if (beamEnd != null)
                {
                    Destroy(beamEnd);
                    beamEnd = null;
                }
            }
            else
            {
            }
            boxCollider.enabled = false;
        }

        if(activationsSequence.Count == 0)
        {
            if(isActive)
            {
                beamActivationState += beamChangeStateSpeed * Time.deltaTime;
                beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                beamMaterial.SetFloat("_laserSwitch", beamActivationState);
            }
            else
            {
                beamActivationState -= beamChangeStateSpeed * Time.deltaTime;
                beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                beamMaterial.SetFloat("_laserSwitch", beamActivationState);
            }
            beamMaterial.SetFloat("_previsOrAttack", 1);
        }
    }

    private void UpdateSequence()
    {
        if(isActive)
        {
            if (elapsedSequenceTime > activationsSequence[currentSequenceIndex])
            {
                elapsedSequenceTime = 0;
                currentSequenceIndex++;
                if (currentSequenceIndex >= activationsSequence.Count)
                {
                    currentSequenceIndex = 0;
                }

                if (oddIndexActive ? currentSequenceIndex % 2 != 0 : currentSequenceIndex % 2 == 0)
                {
                    isBeamActive = true;
                }
                else
                {
                    isBeamActive = false;
                }
            }

            if(!isBeamActive)
            {
                if (elapsedSequenceTime > activationsSequence[currentSequenceIndex] - beamWarningTime)
                {
                    beamActivationState += beamChangeStateSpeed * Time.deltaTime;
                    beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                    beamMaterial.SetFloat("_laserSwitch", beamActivationState);
                }
                else
                {
                    beamActivationState -= beamChangeStateSpeed * Time.deltaTime;
                    beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                    beamMaterial.SetFloat("_laserSwitch", beamActivationState);
                }

                if (beamActivationState == 0)
                {
                    beamState = 0;
                    beamMaterial.SetFloat("_previsOrAttack", beamState);
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
        hit = Physics2D.Raycast(transform.position, GetDirectionFromAngle(transform.rotation.eulerAngles.z), maxLaserRange, LayerMask.GetMask("Wall"));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, hit ? hit.point : (Vector2)transform.position + GetDirectionFromAngle(transform.rotation.eulerAngles.z) * maxLaserRange);
    }
}
