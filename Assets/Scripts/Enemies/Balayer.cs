using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balayer : Enemy
{
    [Header("Movement settings")]
    public float maxSpeed;
    public float accelerationForce;
    public float safeDistance;
    public float safeDistanceWidth;
    [Header("Beam settings")]
    public float provocationRange;
    public int beamDamage;
    public float beamKnockback;
    public float aimTime;
    public float chargeTime;
    public float beamTime;
    public float beamMaxRotationSpeed;
    public float beamRotationAcceleration;
    public float beamCoolDown;
    public float beamStartAngleOffset;
    public float beamNoMovementTime;
    public float beamEndSlowingTime;
    public GameObject beamStartPrefab;
    public GameObject beamPartPrefab;
    public GameObject beamImpactPrefab;
    [Header("Technical settings")]
    public float seekingBeamSpotAngleInterval;
    public LineRenderer beamWarningLine;
    public float spaceBetweenBeamFx;
    public float beamStartOffset;
    public Transform beamParent;

    private bool isShooting;
    private bool isAiming;
    private bool playerInSight;
    private bool destinationReached;
    private bool canBeInSight;
    private Vector2 potentialTargetPos;
    private float elapsedAimTime;
    private float beamCoolDownElapsed;
    private Vector2 playerDirection;

    new void Start()
    {
        base.Start();
        provoked = false;
        isShooting = false;
        destinationReached = false;
        playerInSight = false;
        canBeInSight = true;
        elapsedAimTime = 0;
        beamCoolDownElapsed = 0;
        beamWarningLine.enabled = false;
    }

    new void Update()
    {
        base.Update();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateMovement()
    {
        if(provoked)
        {
            if (path != null && !pathEndReached && !destinationReached && inControl && canBeInSight && !isShooting && !isAiming)
            {
                Vector2 force = new Vector2(pathDirection.x * accelerationForce, pathDirection.y * accelerationForce);

                rb.velocity += force * Time.fixedDeltaTime;

                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }
            else if (destinationReached)
            {
                rb.velocity -= rb.velocity.normalized * accelerationForce * Time.fixedDeltaTime;
                if (rb.velocity.magnitude <= accelerationForce * Time.fixedDeltaTime)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        playerInSight = IsPlayerInSightFrom(transform.position);
        playerDirection = GameData.player.transform.position - transform.position;
        playerDirection.Normalize();
        destinationReached = distToPlayer >= safeDistance && distToPlayer < safeDistance + safeDistanceWidth && playerInSight;
        if (provoked)
        {
            if (!destinationReached && !isShooting)
            {
                isAiming = false;
                elapsedAimTime = 0;
                potentialTargetPos = FindNearestBeamSpot();
                if(potentialTargetPos != (Vector2)transform.position)
                {
                    targetPathfindingPosition = potentialTargetPos;
                    canBeInSight = true;
                }
                else
                {
                    canBeInSight = false;
                }
            }
            else
            {
                if(!isShooting && beamCoolDownElapsed > beamCoolDown)
                {

                    if (elapsedAimTime > aimTime)
                    {
                        isAiming = false;
                        StartCoroutine(ShootBeam());
                    }
                    else
                    {
                        isAiming = true;
                        elapsedAimTime += Time.deltaTime;
                    }
                }
                else
                {
                    isAiming = false;
                    elapsedAimTime = 0;
                }
            }
        }
        else
        {
            provoked = distToPlayer < provocationRange;
        }


        if (beamCoolDownElapsed < beamCoolDown)
        {
            beamCoolDownElapsed += Time.deltaTime;
        }
    }

    private Vector2 FindNearestBeamSpot()
    {
        Vector2 spot = transform.position;
        bool validSpotFound = false;
        float nearestAngle = Vector2.SignedAngle(Vector2.right, transform.position - GameData.player.transform.position);
        nearestAngle /= seekingBeamSpotAngleInterval;
        nearestAngle = Mathf.Round(nearestAngle) * seekingBeamSpotAngleInterval;
        float angleSeekOffset = 0;
        int iteration = 0;
        bool seekPositive = true;
        float seekAngle = 0;
        Vector2 seekDirection;
        while(angleSeekOffset < 181 && !validSpotFound && seekingBeamSpotAngleInterval != 0)
        {
            angleSeekOffset = seekingBeamSpotAngleInterval * iteration;
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
            spot = (Vector2)GameData.player.transform.position + seekDirection * safeDistance;
            if(IsPlayerInSightFrom(spot))
            {
                validSpotFound = true;
            }
            Debug.DrawRay(spot, Vector2.up * 0.1f, validSpotFound ? Color.green : Color.white, 0.02f);
        }

        return validSpotFound ? spot : (Vector2)transform.position;
    }

    private Vector2 DirectionFromAngle(float angle)
    {
        Vector2 direction;
        direction.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        direction.y = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.Normalize();
        return direction;
    }

    private IEnumerator ShootBeam()
    {
        elapsedAimTime = 0;
        isShooting = true;
        beamCoolDownElapsed = 0;
        float elapsedBeamTime = 0;
        Vector2 startDirection = GameData.player.transform.position - transform.position;
        startDirection.Normalize();
        float shootAngle = Vector2.SignedAngle(Vector2.right, startDirection);
        shootAngle += Random.Range(0, 2) == 0 ? beamStartAngleOffset : -beamStartAngleOffset;
        float currentRotSpeed = 0;
        yield return new WaitForSeconds(chargeTime);
        bool rotPositive = Vector2.SignedAngle(Vector2.right, playerDirection) - shootAngle > 0;

        List<GameObject> beamFxs = new List<GameObject>();
        GameObject beamEnd = null;
        float beamLength;
        int beamFxNumber;

        while (elapsedBeamTime < beamTime)
        {
            if(elapsedBeamTime > beamTime - beamEndSlowingTime)
            {
                if(Mathf.Abs(currentRotSpeed) > 0.01f)
                {
                    currentRotSpeed -= beamRotationAcceleration * Time.fixedDeltaTime * Mathf.Sign(currentRotSpeed);
                }
                else
                {
                    currentRotSpeed = 0;
                }
            }
            else if(elapsedBeamTime > beamNoMovementTime)
            {
                currentRotSpeed += beamRotationAcceleration * Time.fixedDeltaTime * (rotPositive ? 1 : -1);
                currentRotSpeed = Mathf.Clamp(currentRotSpeed, -beamMaxRotationSpeed, beamMaxRotationSpeed);
            }
            else
            {
                currentRotSpeed = 0;
            }
            shootAngle += currentRotSpeed;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, DirectionFromAngle(shootAngle), 100.0f, LayerMask.GetMask("Wall"));
            //Debug.DrawLine(transform.position, hit ? hit.point : (Vector2)transform.position + DirectionFromAngle(shootAngle) * 100, Color.red);

            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, DirectionFromAngle(shootAngle), 100.0f, LayerMask.GetMask("Player","Wall"));
            if(playerHit && playerHit.collider.CompareTag("Player"))
            {
                GameData.playerManager.LoseSpiritParts(beamDamage, DirectionFromAngle(shootAngle) * beamKnockback);
            }

            beamLength = hit ? Vector2.Distance(transform.position, hit.point) - beamStartOffset : 30;
            beamFxNumber = Mathf.CeilToInt(beamLength / spaceBetweenBeamFx);

            beamParent.rotation = Quaternion.identity;

            if (beamFxs.Count < beamFxNumber)
            {
                for (int i = beamFxs.Count; i < beamFxNumber; i++)
                {
                    beamFxs.Add(Instantiate(i == 0 ? beamStartPrefab : beamPartPrefab, (Vector2)transform.position + Vector2.right * ((spaceBetweenBeamFx * i) + beamStartOffset), Quaternion.identity, beamParent));
                }
            }
            else if(beamFxs.Count > beamFxNumber)
            {
                for (int i = beamFxs.Count - 1; i > beamFxNumber - 1; i--)
                {
                    Destroy(beamFxs[i]);
                    beamFxs.RemoveAt(i);
                }
            }

            beamParent.rotation = Quaternion.Euler(0, 0, shootAngle);

            if(beamEnd == null)
            {
                beamEnd = Instantiate(beamImpactPrefab, transform.position, Quaternion.identity);
            }
            beamEnd.transform.position = hit ? hit.point : (Vector2)transform.position + DirectionFromAngle(shootAngle) * beamLength;

            yield return new WaitForFixedUpdate();
            elapsedBeamTime += Time.fixedDeltaTime;
        }
        isShooting = false;

        for (int i = beamFxs.Count - 1; i >= 0; i--)
        {
            Destroy(beamFxs[i]);
            beamFxs.RemoveAt(i);
        }
        Destroy(beamEnd);
    }
}
