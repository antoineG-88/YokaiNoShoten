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
    public float slowingForce;
    public float maxTractionSpeed;
    public float noGravityMaxTractionSpeed;
    public float startTractionPropulsion;
    public float minDistanceToAccelerate;
    [Space]
    [Range(0, 100)] public float velocityKeptReleasingHook;
    public float ropeBreakParticleByUnit;
    public GameObject breakRopeParticle;
    [Space]
    [Header("AutoAim settings")]
    public float aimAssistAngle;
    public float aimAssistRaycastNumber;
    public float angleToTakeClosest;
    [Header("Key bindings settings")]
    public bool aimWithLeftJoystick;
    public bool tractWithLeftTrigger;
    [Space]
    [Header("References")]
    public GameObject armShoulderO;
    public Transform shootPoint;
    public GameObject ringHighLighterO;
    public GameObject grapplingStartPointO;
    [Header("Debug settings")]
    public bool displayAutoAimRaycast;

    [HideInInspector] public Rigidbody2D rb;
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
    [HideInInspector] public bool isSucked;

    private bool tractTriggerDown;
    private bool tractTriggerPressed;

    //pour corriger le stretch visuel du material du grappin
    private float distanceRopeRing;

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
        tractTriggerDown = false;
        tractTriggerPressed = false;
    }

    private void Update()
    {
        TractTriggerUpdate();
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
            Vector2 aimStickMag = aimWithLeftJoystick ? new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV")) : new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV"));
            if (!isAiming && aimStickMag.magnitude > 0.1f && !isTracting)
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
                    aimDirection = aimWithLeftJoystick ? new Vector2(Input.GetAxis("LeftStickH"), -Input.GetAxis("LeftStickV")) : new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV"));
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

                    hit = Physics2D.Raycast(raycastOrigin, direction, maxGrappleRange, LayerMask.GetMask("Ring", "Wall", "Enemy"));
                    if (hit)
                    {
                        if ((LayerMask.LayerToName(hit.collider.gameObject.layer) != "Wall") && selectedObject != hit.collider.gameObject && Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y)) < minAngleFound && GameData.cameraHandler.IsPointInCameraView(hit.collider.transform.position, 0.8f))
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
                    ringHighLighterO.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, -shootDirection));
                }
                else
                {
                    shootDirection = aimDirection;
                    shootDirection.y *= -1;
                    ringHighLighterO.SetActive(false);
                }

                if (tractTriggerDown && timeBeforeNextShoot <= 0 && !isHooked)
                {
                    timeBeforeNextShoot = shootCooldown;
                    ReleaseHook();

                    if (selectedObject != null)
                    {
                        Vector2 selectedObjectDirection = selectedObject.transform.position - transform.position;
                        selectedObjectDirection.Normalize();
                        selectedRing = selectedObject.GetComponent<Ring>();
                        if (selectedRing != null)
                        {
                            selectedRing.AttachReaction();
                            if (selectedRing.attachable == true)
                            {
                                AttachHook(selectedObject);
                            }
                        }
                        else
                        {
                            AttachHook(selectedObject);
                        }

                        RaycastHit2D antiGrabWallHit = Physics2D.Raycast(transform.position, selectedObjectDirection, Vector2.Distance(selectedObject.transform.position, transform.position), LayerMask.GetMask("AntiGrabWall"));
                        if (antiGrabWallHit)
                        {
                            BreakRope("AntigrabWall traversed");
                        }
                        if (isSucked)
                        {
                            BreakRope("nope u suck");
                        }
                    }
                }
            }
            else
            {
                ringHighLighterO.SetActive(false);
            }
        }
        else
        {
            ringHighLighterO.SetActive(false);
            armShoulderO.SetActive(false);
            isAiming = false;
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
            ropePoints[1] = transform.position;
            ropePoints[0] = attachedObject.transform.position;
            ropeRenderer.SetPositions(ropePoints);

            //pour corriger le stretch visuel du material du grappin
            distanceRopeRing = Vector3.Distance(transform.position, attachedObject.transform.position);
            ropeRenderer.material.mainTextureScale = new Vector2(distanceRopeRing*2,1);
            
            if (canUseTraction && tractTriggerPressed && !GameData.dashHandler.isDashing)
            {
                GameData.movementHandler.canMove = false;
                armShoulderO.SetActive(false);
                isAiming = false;

                if (!isTracting)
                {
                    float startTractionVelocity = Mathf.Cos(Vector2.SignedAngle(rb.velocity, tractionDirection) * Mathf.Deg2Rad) * rb.velocity.magnitude;

                    if (startTractionVelocity < 0)
                    {
                        startTractionVelocity = 0;
                    }

                    startTractionVelocity += startTractionPropulsion;
                    rb.velocity = startTractionVelocity * tractionDirection;
                }
                rb.velocity = rb.velocity.magnitude * tractionDirection;

                if (rb.velocity.magnitude > (GameData.movementHandler.currentGravityZone != null ? noGravityMaxTractionSpeed : maxTractionSpeed))
                {
                    if (rb.velocity.magnitude - slowingForce * Time.fixedDeltaTime > (GameData.movementHandler.currentGravityZone != null ? noGravityMaxTractionSpeed : maxTractionSpeed))
                    {
                        rb.velocity -= tractionDirection * slowingForce * Time.fixedDeltaTime;
                    }
                    else
                    {
                        rb.velocity = tractionDirection * (GameData.movementHandler.currentGravityZone != null ? noGravityMaxTractionSpeed : maxTractionSpeed);
                    }
                }
                else if ((rb.velocity.magnitude + 0.1f) < startTractionPropulsion && startTractionPropulsion < (GameData.movementHandler.currentGravityZone != null ? noGravityMaxTractionSpeed : maxTractionSpeed))
                {
                    rb.velocity = tractionDirection * startTractionPropulsion;
                }
                else
                {
                    if (Vector2.Distance(transform.position, attachedObject.transform.position) <= minDistanceToAccelerate)
                    {
                        rb.velocity += tractionDirection * tractionForce * Time.fixedDeltaTime;
                    }
                }

                isTracting = true;
            }

            if (isTracting && (!tractTriggerPressed || Vector2.Distance(transform.position, attachedObject.transform.position) < releasingHookDist))
            {
                rb.velocity *= velocityKeptReleasingHook / 100;
                isTracting = false;

                ReleaseHook();
            }
        }
        else
        {
            canShoot = true;
            ropeRenderer.enabled = false;
            if (attachedObject == null)
            {
                //ReleaseHook();
            }
        }
    }

    private void HookManager()
    {
        if (isHooked)
        {
            if (selectedRing != null && selectedRing.attachable == false)
            {
                BreakRope("Not attachable");
                rb.velocity *= velocityKeptReleasingHook / 100;
            }

            RaycastHit2D ringHit = Physics2D.Raycast(transform.position, tractionDirection, maxGrappleRange, LayerMask.GetMask("Ring", "Enemy", "Wall", "AntiGrabWall"));
            if (ringHit && (LayerMask.LayerToName(ringHit.collider.gameObject.layer) == "Wall" || LayerMask.LayerToName(ringHit.collider.gameObject.layer) == "AntiGrabWal"))
            {
                BreakRope("hit a wall");
                rb.velocity *= velocityKeptReleasingHook / 100;
            }
        }
    }

    public void AttachHook(GameObject objectToAttach)
    {
        isHooked = true;
        //GameData.dashHandler.isReaiming = false;
        GameData.pierceHandler.StopPhasingTime();
        attachedObject = objectToAttach;
        tractionDirection = (attachedObject.transform.position - transform.position);
        tractionDirection.Normalize();
        if (isSucked == false)
        {
            GameData.dashHandler.canDash = true;
        }
        GameData.pierceHandler.canPierce = true;
    }

    public void ReleaseHook()
    {
        isHooked = false;
        isTracting = false;
        ropeRenderer.enabled = false;
        GameData.movementHandler.canMove = true;
        attachedObject = null;
        //GameData.movementHandler.isAffectedbyGravity = true;
    }

    public void BreakRope(string message)
    {
        if (attachedObject != null)
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

    private void TractTriggerUpdate()
    {
        if (!tractTriggerPressed && (tractWithLeftTrigger ? Input.GetAxisRaw("LeftTrigger") == 1 : Input.GetAxisRaw("RightTrigger") == 1))
        {
            tractTriggerDown = true;
        }
        else
        {
            tractTriggerDown = false;
        }

        if (tractWithLeftTrigger ? Input.GetAxisRaw("LeftTrigger") == 1 : Input.GetAxisRaw("RightTrigger") == 1)
        {
            tractTriggerPressed = true;
        }
        else
        {
            tractTriggerPressed = false;
            tractTriggerDown = false;
        }
    }
}
