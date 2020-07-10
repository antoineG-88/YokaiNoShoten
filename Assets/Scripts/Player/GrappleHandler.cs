﻿using System.Collections;
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
    private GameObject selectedRing;
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

        if (canShoot)
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


                selectedRing = null;

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

                    hit = Physics2D.Raycast(raycastOrigin, direction, maxGrappleRange, LayerMask.GetMask("Ring", "Wall"));
                    if (hit && hit.collider.CompareTag("Ring") && selectedRing != hit.collider.gameObject && Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y)) < minAngleFound)
                    {
                        selectedRing = hit.collider.gameObject;
                        minAngleFound = Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y));
                    }
                }

                if (selectedRing != null)
                {
                    ringHighLighterO.SetActive(true);
                    ringHighLighterO.transform.position = selectedRing.transform.position;
                    shootDirection = new Vector2(selectedRing.transform.position.x - shootPoint.position.x, selectedRing.transform.position.y - shootPoint.position.y).normalized;
                }
                else
                {
                    shootDirection = aimDirection;
                    shootDirection.y *= -1;
                    ringHighLighterO.SetActive(false);
                }

                if (rightTriggerDown && timeBeforeNextShoot <= 0 && !isHooked) //Input changed
                {
                    timeBeforeNextShoot = shootCooldown;
                    ReleaseHook();

                    if (selectedRing != null)
                    {
                        AttachHook(selectedRing);
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
        if (isHooked)
        {
            canShoot = false;
            ringHighLighterO.SetActive(false);
            attachedObject.GetComponent<Collider2D>().isTrigger = true;

            ropeRenderer.enabled = true;
            Vector3[] ropePoints = new Vector3[2];
            ropePoints[0] = transform.position;
            ropePoints[1] = attachedObject.transform.position;
            ropeRenderer.SetPositions(ropePoints);

            tractionDirection = (attachedObject.transform.position - transform.position);
            tractionDirection.Normalize();

            if (canUseTraction && Input.GetAxisRaw("RightTrigger") == 1/* && !GameData.playerMovement.isDashing*/)
            {
                GameData.movementHandler.isAffectedbyGravity = false;
                GameData.movementHandler.inControl = false;

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
            if (isTracting && (Input.GetAxisRaw("RightTrigger") == 0 || ((distance = Vector2.Distance(transform.position, attachedObject.transform.position)) < releasingHookDist)))
            {
                rb.velocity *= velocityKeptReleasingHook / 100;
                isTracting = false;

                //Reset DashAttack cooldown

                ReleaseHook();
            }
        }
        else
        {
            canShoot = true;
            ropeRenderer.enabled = false;
        }
    }

    private void HookManager()
    {
        if (isHooked)
        {
            RaycastHit2D ringHit = Physics2D.Raycast(transform.position, tractionDirection, maxGrappleRange, LayerMask.GetMask("Ring", "Wall"));
            if (ringHit && !ringHit.collider.CompareTag("Ring"))
            {
                BreakRope();
            }
        }
    }

    public void AttachHook(GameObject objectToAttach)
    {
        isHooked = true;
        attachedObject = objectToAttach;
    }

    public void ReleaseHook()
    {
        isHooked = false;
        isTracting = false;
        ropeRenderer.enabled = false;
        GameData.movementHandler.inControl = true;
        attachedObject = null;
        GameData.movementHandler.isAffectedbyGravity = true;
    }

    public void BreakRope()
    {
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
