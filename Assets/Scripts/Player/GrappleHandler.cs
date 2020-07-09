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
    public Transform shootPoint;

    private Vector2 aimDirection;
    private GameObject selectedRing;
    private bool isAiming;
    private bool isAttached;
    private float aimAssistSubAngle;
    private float aimAssistFirstAngle;

    void Start()
    {
        aimAssistFirstAngle = -aimAssistAngle / 2;
        aimAssistSubAngle = aimAssistAngle / (aimAssistRaycastNumber - 1);
        selectedRing = null;
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
            selectedRing = null;
            for (int i = 0; i < aimAssistRaycastNumber; i++)
            {
                float relativeAngle = aimAssistFirstAngle + aimAssistSubAngle * i;
                float angledDirection = (Mathf.Atan2(aimDirection.x, -aimDirection.y) * 180 / Mathf.PI - 90) + relativeAngle;
                Vector2 direction = new Vector2(Mathf.Cos((angledDirection) * Mathf.PI / 180), Mathf.Sin((angledDirection) * Mathf.PI / 180));
                Vector2 raycastOrigin = shootPoint.position;
                hit = Physics2D.Raycast(raycastOrigin, direction, maxGrappleRange, LayerMask.GetMask("Ring", "Wall"));
                if (hit && hit.collider.CompareTag("Ring") && selectedRing != hit.collider.gameObject && Vector2.Angle(direction, new Vector2(aimDirection.x, aimDirection.y)) < minAngleFound)
                {
                    selectedRing = hit.collider.gameObject;
                    minAngleFound = Vector2.Angle(direction, new Vector2(aimDirection.x, -aimDirection.y));
                }
                Debug.DrawRay(raycastOrigin, direction * maxGrappleRange, Color.cyan);
            }
        }
        else
        {
            selectedRing = null;
            grappleCursor.SetActive(false);
        }


        if(selectedRing != null)
        {
            grappleSelectIndicator.SetActive(true);
            grappleSelectIndicator.transform.position = selectedRing.transform.position;
            if(Input.GetAxisRaw("RightTrigger") == 1)
            {
                Attach();
            }
        }
        else
        {
            grappleSelectIndicator.SetActive(false);
        }
    }


    private void Attach()
    {
        isAttached = true;

    }
}
