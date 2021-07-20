using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float maxLaserRange;
    public float knockbackDistance;
    public Switch connectedSwitch;
    [Header("Technical settings")]
    public GameObject beamStartPrefab;
    public GameObject beamPartPrefab;
    public GameObject beamImpactPrefab;
    public float spaceBetweenBeamFx;
    public float beamStartOffset;
    public Transform beamParent;

    private Vector2 currentDirection;
    private float beamLength;
    private int beamFxNumber;
    private List<GameObject> beamFxs = new List<GameObject>();
    private GameObject beamEnd = null;
    private RaycastHit2D hit;
    private RaycastHit2D playerHit;

    private bool isActive;
    private bool isBeamActive;

    void Start()
    {
        isActive = true;
        isBeamActive = true;
    }

    void Update()
    {
        currentDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z);

        Debug.DrawRay(transform.position, currentDirection * 100);
    }

    private void FixedUpdate()
    {
        UpdateLaserBeam();
    }

    private void UpdateLaserBeam()
    {
        if(isBeamActive && isActive)
        {
            hit = Physics2D.Raycast(transform.position, currentDirection, maxLaserRange, LayerMask.GetMask("Wall"));

            playerHit = Physics2D.Raycast(transform.position, currentDirection, maxLaserRange, LayerMask.GetMask("Player", "Wall"));
            if (playerHit && playerHit.collider.CompareTag("Player"))
            {
                GameData.playerManager.TakeDamage(1, currentDirection * knockbackDistance);
            }

            beamLength = hit ? Vector2.Distance(transform.position, hit.point) - beamStartOffset : maxLaserRange;
            beamFxNumber = Mathf.CeilToInt(beamLength / spaceBetweenBeamFx);

            beamParent.rotation = Quaternion.identity;

            if (beamFxs.Count < beamFxNumber)
            {
                for (int i = beamFxs.Count; i < beamFxNumber; i++)
                {
                    beamFxs.Add(Instantiate(i == 0 ? beamStartPrefab : beamPartPrefab, (Vector2)transform.position + Vector2.right * ((spaceBetweenBeamFx * i) + beamStartOffset), Quaternion.identity, beamParent));
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
        }
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}
