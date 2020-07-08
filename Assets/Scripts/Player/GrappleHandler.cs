using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHandler : MonoBehaviour
{
    public float maxGrappleRange;
    public float aimAssistAngle;
    public int aimAssistRaycastNumber;

    public LayerMask ringMask;

    public GameObject grappleCursor;
    public GameObject grappleSelectIndicator;

    private Vector2 aimDirection;
    private Ring selectedRing;
    private bool isAiming;
    private bool isAttached;
    private GameObject closestRing;
    private float aimAssistSubAngle;
    private float aimAssistFirstAngle;

    void Start()
    {
        aimAssistFirstAngle = -aimAssistAngle / 2;
        aimAssistSubAngle = aimAssistAngle / (aimAssistRaycastNumber - 1);
        closestRing = null;
     }

    void Update()
    {
        SelectHook();
    }

    private void SelectHook()
    {
        aimDirection = new Vector2(Input.GetAxis("RightStickH"), -Input.GetAxis("RightStickV"));
        if(!isAiming && aimDirection.magnitude > 0.1f)
        {
            isAiming = true;
        }
        else if (isAiming && aimDirection.magnitude <= 0.1f)
        {
            isAiming = false;
        }

        if(isAiming)
        {
            grappleCursor.SetActive(true);
            aimDirection.Normalize();
            grappleCursor.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, aimDirection)));


            RaycastHit2D hit;
            float minAngleFound = aimAssistAngle;
            closestRing = null;
            for (int i = 0; i < aimAssistRaycastNumber; i++)
            {
                float relativeAngle = aimAssistFirstAngle + aimAssistSubAngle * i;
                float angledDirection = (Mathf.Atan2(aimDirection.y, aimDirection.x) * 180 / Mathf.PI - 90) + relativeAngle;
                Vector2 direction = new Vector2(Mathf.Cos((angledDirection + 90) * Mathf.PI / 180), Mathf.Sin((angledDirection + 90) * Mathf.PI / 180));
                hit = Physics2D.Raycast(transform.position, direction, maxGrappleRange, ringMask);
                if (hit && closestRing != hit.collider.gameObject && Vector2.Angle(direction, aimDirection) < minAngleFound)
                {
                    closestRing = hit.collider.gameObject;
                    minAngleFound = Vector2.Angle(direction, aimDirection);
                }
            }
        }
        else
        {
            closestRing = null;
            grappleCursor.SetActive(false);
        }


        if(closestRing != null)
        {
            grappleSelectIndicator.SetActive(true);
            grappleSelectIndicator.transform.position = closestRing.transform.position;
            if(Input.GetAxisRaw("RightTrigger") == 1)
            {
                Attach();
            }
        }
    }


    private void Attach()
    {
        isAttached = true;

    }
}
