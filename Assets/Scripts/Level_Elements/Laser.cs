using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float maxLaserRange;
    public float beamWidth;
    public float beamWallWidth;
    public float knockbackDistance;
    public bool doKnockbackThroughLaser;
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
    public Material sequenceMat;
    public float disablingDistance;
    public bool doNotUseDistanceDisabling;
    [Header("Sounds")]
    public AudioSource laserLoopSource;
    public AudioSource source;
    public Sound startSound;
    public Sound stopSound;

    public ParticleSystem endBeamParticle;
    public Color sequencialColor;

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
    private Vector2 distToPlayer;

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
            laserLoopSource.Play();
        }
        beamLine = GetComponent<LineRenderer>();
        if(activationsSequence.Count > 0)
        {
            beamMaterial = Instantiate(sequenceMat);
            ParticleSystem.MainModule beamEndParticleMain = endBeamParticle.main;
            beamEndParticleMain.startColor = sequencialColor;
        }
        else
        {
            beamMaterial = Instantiate(beamLine.sharedMaterial);
        }
        beamLine.sharedMaterial = beamMaterial;
    }

    void Update()
    {
        currentDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z);
        if(connectedSwitch != null)
        {
            if(isActive != connectedSwitch.IsON())
            {
                isActive = connectedSwitch.IsON();
                if(!isActive)
                {
                    laserLoopSource.Stop();
                }
            }
        }

        if (activationsSequence.Count > 0)
        {
            UpdateSequence();
        }


        if (distToPlayer.magnitude < disablingDistance || doNotUseDistanceDisabling)
        {
            hit = Physics2D.Raycast((Vector2)transform.position + currentDirection * beamStartOffset, currentDirection, maxLaserRange - beamStartOffset, LayerMask.GetMask("Wall"));
            beamLength = hit ? (Vector2.Distance(transform.position, hit.point) - beamDisplayStartOffset) : maxLaserRange;
            beamLine.SetPosition(0, (Vector2)transform.position + currentDirection * beamDisplayStartOffset);
            beamLine.SetPosition(1, hit ? hit.point : (Vector2)transform.position + currentDirection * beamLength);
        }
    }

    private void FixedUpdate()
    {
        distToPlayer = (Vector2)GameData.player.transform.position - (Vector2)transform.position;
        if (distToPlayer.magnitude < disablingDistance || doNotUseDistanceDisabling)
        {
            UpdateLaserBeam();
        }

        source.pitch = Time.timeScale;
        laserLoopSource.pitch = Time.timeScale;
    }

    private void UpdateLaserBeam()
    {
        if (isBeamActive && isActive)
        {
            playerHit = Physics2D.CircleCast((Vector2)transform.position + currentDirection * (beamStartOffset + beamWidth / 2), beamWidth, currentDirection, maxLaserRange - (beamStartOffset + beamWidth), LayerMask.GetMask("Player", "Wall"));
            if (playerHit && playerHit.collider.CompareTag("Player"))
            {
                Vector2 knockbackDirection;
                if(Vector2.SignedAngle(currentDirection, GameData.player.transform.position - transform.position) > 0)
                {
                    knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z + (doKnockbackThroughLaser ? -90 : 90));
                }
                else
                {
                    knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z + (doKnockbackThroughLaser ? 90 : -90));
                }
                GameData.dashHandler.isDashing = false;
                GameData.playerManager.TakeDamage(1, knockbackDirection * knockbackDistance);
            }

            if(!endBeamParticle.isPlaying)
            {
                endBeamParticle.Play();
            }

            endBeamParticle.transform.position = hit.point;
            endBeamParticle.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, hit.normal));

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

            if(!doKnockbackThroughLaser)
            {
                boxCollider.enabled = true;
                boxCollider.offset = new Vector2(beamStartOffset + (beamLength / 2), 0);
                boxCollider.size = new Vector2(beamLength, beamWallWidth);
            }


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

            if (endBeamParticle.isPlaying)
            {
                endBeamParticle.Stop();
            }
        }

        if (isActive)
        {
            if(activationsSequence.Count == 0)
            {
                beamActivationState += beamChangeStateSpeed * Time.deltaTime;
                beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                beamMaterial.SetFloat("_laserSwitch", beamActivationState);
                beamMaterial.SetFloat("_previsOrAttack", 1);
            }
        }
        else
        {
            beamActivationState -= beamChangeStateSpeed * Time.deltaTime;
            beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
            beamMaterial.SetFloat("_laserSwitch", beamActivationState);
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
                    if(!isBeamActive)
                    {
                        isBeamActive = true;
                        laserLoopSource.Play();
                        source.PlayOneShot(startSound.clip, startSound.volumeScale);
                    }
                }
                else
                {
                    if (isBeamActive)
                    {
                        isBeamActive = false;
                        laserLoopSource.Stop();
                        //source.PlayOneShot(stopSound.clip, stopSound.volumeScale);
                    }
                }
            }

            if(!isBeamActive)
            {
                if (elapsedSequenceTime > activationsSequence[currentSequenceIndex] - beamWarningTime)
                {
                    beamActivationState += beamChangeStateSpeed * Time.deltaTime;
                    beamActivationState = Mathf.Clamp(beamActivationState, 0f, 1f);
                    beamMaterial.SetFloat("_laserSwitch", beamActivationState);

                    if (activationsSequence[currentSequenceIndex] <= 1)
                    {
                        beamState -= beamChangeStateSpeed * Time.deltaTime;
                        beamState = Mathf.Clamp(beamState, 0f, 1f);
                        beamMaterial.SetFloat("_previsOrAttack", beamState);
                    }
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
        hit = Physics2D.Raycast(transform.position, GetDirectionFromAngle(transform.rotation.eulerAngles.z), maxLaserRange, LayerMask.GetMask("Wall"), LayerMask.GetMask("DashWall"));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, hit ? hit.point : (Vector2)transform.position + GetDirectionFromAngle(transform.rotation.eulerAngles.z) * maxLaserRange);
    }
}
