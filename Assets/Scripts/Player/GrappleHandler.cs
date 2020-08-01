using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHandler : MonoBehaviour
{
    [Header("Grapple settings")]
    public float maxGrappleRange;
    public float grappleAttachLengthDifference;
    public float shootCooldown;
    public float releasingHookDist;
    public float minAttachDistance;
    public bool keepAim;
    [Space]
    [Header("Momentum settings")]
    public float tractionForce;
    public float maxTractionSpeed;
    public float startTractionPropulsion;
    [Space]
    [Range(0, 100)] public float velocityKeptReleasingHook;
    public float ropeBreakParticleByUnit;
    public GameObject breakRopeParticle;
    [Space]
    [Header("AutoAim settings")]
    public float aimAssistAngle;
    public float aimAssistRaycastNumber;
    [Space]
    [Header("References")]
    public GameObject armShoulderO;
    public Transform shootPoint;
    public GameObject ringHighLighterO;
    public GameObject grapplingStartPointO;
    [Header("Debug settings")]
    public bool displayAutoAimRaycast;

    private Rigidbody2D rb;
    [HideInInspector] public Vector2 aimDirection;
    private LineRenderer ropeRenderer;
    [HideInInspector] public bool isAiming;
    private Rigidbody2D ringRb;
    [HideInInspector] public float timeBeforeNextShoot;
    private GameObject selectedObject;
    private Ring selectedRing;
    private float aimAssistSubAngle;
    private float aimAssistFirstAngle;
    private Vector2 shootDirection;
    [HideInInspector] public bool isHooked;
    [HideInInspector] public GameObject attachedObject;
    [HideInInspector] public Vector2 tractionDirection;
    [HideInInspector] public bool isTracting;
    [HideInInspector] public bool canUseTraction;
    [HideInInspector] public bool canShoot;

    private bool rightTriggerDown;
    private bool rightTriggerPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ropeRenderer = GetComponent<LineRenderer>();
        ringHighLighterO.SetActive(false);
        isAiming = false;
        isHooked = false;
        isTracting = false;
        canUseTraction = true;
        canShoot = true;
        armShoulderO.SetActive(false);
        timeBeforeNextShoot = 0;
        aimAssistSubAngle = aimAssistAngle / (aimAssistRaycastNumber - 1);
        aimAssistFirstAngle = -aimAssistAngle / 2;
        rightTriggerDown = false;
        rightTriggerPressed = false;
    }

    private void Update()
    {
        RightTriggerUpdate();
        AimManager();
    }

    private void FixedUpdate()
    {
        TractionManager();

        HookManager();
    }

    void AimManager()
    {
        if (timeBeforeNextShoot > 0)
        {
            timeBeforeNextShoot -= Time.deltaTime;
        }

        if (canShoot && GameData.playerManager.inControl)
        {
            Vector2 aimStickMag = new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV"));
            if (!isAiming && aimStickMag.magnitude > 0.1f)
            {
                isAiming = true;
                armShoulderO.SetActive(true);
            }
            else if (isAiming && aimStickMag.magnitude <= 0.1f)
            {
                isAiming = false;
                if (!keepAim)
                {
                    armShoulderO.SetActive(false);
                }
            }

            if (isAiming || keepAim)
            {
                if (isAiming)
                {
                    aimDirection = new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV"));
                    aimDirection.Normalize();
                    armShoulderO.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.up, aimDirection));
                }


                selectedObject = null;

                RaycastHit2D hit;
                float minAngleFound = aimAssistAngle;
                for (int i = 0; i < aimAssistRaycastNumber; i++)
                {
                    float relativeAngle = aimAssistFirstAngle + aimAssistSubAngle * i;
                    float angledDirection = (Vector2.SignedAngle(Vector2.right, aimDirection)) + relativeAngle;
                    Vector2 direction = new Vector2(Mathf.Cos((angledDirection) * Mathf.PI / 180), Mathf.Sin((angledDirection) * Mathf.PI / 180));
                    Vector2 raycastOrigin = shootPoint.position;

                    raycastOrigin = (Vector2)transform.position + direction * minAttachDistance;

                    if (displayAutoAimRaycast)
                    {
                        Debug.DrawRay(raycastOrigin, direction * maxGrappleRange, Color.cyan);
                    }

                    hit = Physics2D.Raycast(raycastOrigin, direction, maxGrappleRange, LayerMask.GetMask("Ring", "Wall", "Enemy", "SpiritPart"));
                    if (hit)
                    {
                        if ((LayerMask.LayerToName(hit.collider.gameObject.layer) != "Wall") && selectedObject != hit.collider.gameObject && Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y)) < minAngleFound)
                        {
                            selectedObject = hit.collider.gameObject;
                            minAngleFound = Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y));
                        }
                    }
                }


                Vector3[] ropePoints = new Vector3[2];

                RaycastHit2D ropeHit = Physics2D.Raycast(transform.position, aimDirection, maxGrappleRange, LayerMask.GetMask("Ring", "Wall", "Enemy", "SpiritPart"));
                ropePoints[0] = transform.position;
                if (ropeHit)
                {
                    ropePoints[1] = ropeHit.point;
                    ringHighLighterO.SetActive(false);
                    ringHighLighterO.transform.position = ropeHit.point;
                }
                else
                {
                    ropePoints[1] = (Vector2)transform.position + aimDirection * maxGrappleRange;
                }

                ropeRenderer.enabled = false;
                ropeRenderer.SetPositions(ropePoints);

                if (selectedObject != null)
                {
                    ringHighLighterO.SetActive(true);
                    ringHighLighterO.transform.position = selectedObject.transform.position;
                    shootDirection = new Vector2(selectedObject.transform.position.x - shootPoint.position.x, selectedObject.transform.position.y - shootPoint.position.y).normalized;
                }
                else
                {
                    shootDirection = aimDirection;
                    shootDirection.y *= -1;
                    ringHighLighterO.SetActive(false);
                }

                if (rightTriggerDown && timeBeforeNextShoot <= 0 && !isHooked)
                {
                    timeBeforeNextShoot = shootCooldown;
                    ReleaseHook();

                    if (selectedObject != null)
                    {
                        Vector2 selectedObjectDirection = selectedObject.transform.position - transform.position;
                        selectedObjectDirection.Normalize();
                        selectedRing = selectedObject.GetComponent<Ring>();
                        if(selectedRing != null)
                        {
                            if(selectedRing.attachable == true)
                            {
                                AttachHook(selectedObject);
                            }
                        }
                        else
                        {
                            if(selectedObject.CompareTag("Enemy"))
                            {
                                AttachHook(selectedObject);

                                AntiGrabShieldHandler attachedEnemyShield = selectedObject.GetComponent<AntiGrabShieldHandler>();

                                if (!attachedEnemyShield.CanBeGrappledFrom(selectedObjectDirection))
                                {
                                    BreakRope("cannot attach from this side");
                                }
                            }
                            else
                            {
                                AttachHook(selectedObject);
                            }
                        }

                        RaycastHit2D antiGrabWallHit = Physics2D.Raycast(transform.position, selectedObjectDirection, Vector2.Distance(selectedObject.transform.position, transform.position), LayerMask.GetMask("AntiGrabWall"));
                        if(antiGrabWallHit)
                        {
                            BreakRope("AntigrabWall traversed");
                        }
                    }
                }
            }
            else
            {
                ringHighLighterO.SetActive(false);
            }
        }
    }

    void TractionManager()
    {
        if (isHooked && attachedObject != null)
        {
            canShoot = false;
            tractionDirection = (attachedObject.transform.position - transform.position);
            tractionDirection.Normalize();
            ringHighLighterO.SetActive(false);

            ropeRenderer.enabled = true;
            Vector3[] ropePoints = new Vector3[2];
            ropePoints[0] = transform.position;
            ropePoints[1] = attachedObject.transform.position;
            ropeRenderer.SetPositions(ropePoints);

            if (canUseTraction && Input.GetAxisRaw("RightTrigger") == 1 && !GameData.dashHandler.isDashing)
            {
                GameData.movementHandler.isAffectedbyGravity = false;
                GameData.movementHandler.canMove = false;

                if (!isTracting)
                {
                    float startTractionVelocity = GameData.movementHandler.isGrounded ? 0 : rb.velocity.magnitude;
                    if (startTractionVelocity < 0)
                    {
                        startTractionVelocity = 0;
                    }

                    startTractionVelocity += startTractionPropulsion;
                    rb.velocity = startTractionVelocity * tractionDirection;
                }

                float tractionDirectionAngle = Mathf.Atan(tractionDirection.y / tractionDirection.x);
                if (tractionDirection.x < 0)
                {
                    tractionDirectionAngle += Mathf.PI;
                }
                else if (tractionDirection.y < 0 && tractionDirection.x > 0)
                {
                    tractionDirectionAngle += 2 * Mathf.PI;
                }
                rb.velocity = new Vector2(rb.velocity.magnitude * Mathf.Cos(tractionDirectionAngle), rb.velocity.magnitude * Mathf.Sin(tractionDirectionAngle));

                rb.velocity += tractionDirection * tractionForce * Time.fixedDeltaTime;

                if (rb.velocity.magnitude > maxTractionSpeed)
                {
                    rb.velocity = tractionDirection * maxTractionSpeed;
                }
                else if (rb.velocity.magnitude < startTractionPropulsion)
                {
                    rb.velocity = tractionDirection * startTractionPropulsion;
                }

                isTracting = true;
            }

            float distance = 10;
            if (isTracting && (!rightTriggerPressed || ((distance = Vector2.Distance(transform.position, attachedObject.transform.position)) < releasingHookDist)))
            {
                rb.velocity *= velocityKeptReleasingHook / 100;
                isTracting = false;

                GameData.dashHandler.canDash = true;

                ReleaseHook();
            }
        }
        else
        {
            canShoot = true;
            ropeRenderer.enabled = false;
            if(attachedObject == null)
            {
                ReleaseHook();
            }
        }
    }

    private void HookManager()
    {
        if (isHooked)
        {
            if(selectedRing!= null && selectedRing.attachable == false)
            {
                BreakRope("Not attachable");
                rb.velocity *= velocityKeptReleasingHook / 100;
            }

            RaycastHit2D ringHit = Physics2D.Raycast(transform.position, tractionDirection, maxGrappleRange, LayerMask.GetMask("Ring", "Enemy", "Wall"));
            if (ringHit && LayerMask.LayerToName(ringHit.collider.gameObject.layer) == "Wall")
            {
                BreakRope("hit a wall");
                rb.velocity *= velocityKeptReleasingHook / 100;
            }
        }
    }

    public void AttachHook(GameObject objectToAttach)
    {
        isHooked = true;
        GameData.dashHandler.isReaiming = false;
        attachedObject = objectToAttach;
        tractionDirection = (attachedObject.transform.position - transform.position);
        tractionDirection.Normalize();
    }

    public void ReleaseHook()
    {
        isHooked = false;
        isTracting = false;
        ropeRenderer.enabled = false;
        GameData.movementHandler.canMove = true;
        attachedObject = null;
        GameData.movementHandler.isAffectedbyGravity = true;
    }

    public void BreakRope(string message)
    {
        if(attachedObject != null)
        {
            int ropeBreakParticleNumber = Mathf.CeilToInt(Vector2.Distance(transform.position, attachedObject.transform.position) * ropeBreakParticleByUnit);
            float lerpUnit = 1 / (float)ropeBreakParticleNumber;
            for (int i = 0; i < ropeBreakParticleNumber; i++)
            {
                Instantiate(breakRopeParticle, Vector2.Lerp(transform.position, attachedObject.transform.position, lerpUnit * i), Quaternion.identity);
            }
        }
        //Debug.Log(message);
        ReleaseHook();
    }

    private void RightTriggerUpdate()
    {
        if(!rightTriggerPressed && Input.GetAxisRaw("RightTrigger") == 1)
        {
            rightTriggerDown = true;
        }
        else
        {
            rightTriggerDown = false;
        }

        if(Input.GetAxisRaw("RightTrigger") == 1)
        {
            rightTriggerPressed = true;
        }
        else
        {
            rightTriggerPressed = false;
            rightTriggerDown = false;
        }
    }
}
