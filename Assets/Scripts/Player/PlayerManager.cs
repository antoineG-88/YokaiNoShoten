﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Health settings")]
    public int maxSpiritPoint;
    public Text healthText;
    public GameObject spiritPartPrefab;
    public Vector2 spiritLossVelocityRange;

    private int currentSpiritPoint;
    private List<SpiritPart> spiritParts = new List<SpiritPart>();

    void Start()
    {
        currentSpiritPoint = maxSpiritPoint;
    }

    void Update()
    {
        healthText.text = currentSpiritPoint.ToString() + " / " + maxSpiritPoint.ToString();
    }

    public void LoseSpiritParts(int damage, Vector2 knockBackDirectedForce)
    {
        currentSpiritPoint -= damage;
        Vector2 initialVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        initialVelocity.Normalize();
        initialVelocity *= Random.Range(spiritLossVelocityRange.x, spiritLossVelocityRange.y);
        SpiritPart newSpiritPart = Instantiate(spiritPartPrefab, transform.position, Quaternion.identity).GetComponent<SpiritPart>();
        newSpiritPart.Initialize(damage, initialVelocity);
        spiritParts.Add(newSpiritPart);
        if (currentSpiritPoint <= 0)
        {
            Debug.Log("You die");
        }
        GameData.playerVisuals.isHurt = 5;
        GameData.movementHandler.Propel(knockBackDirectedForce, false);
        StartCoroutine(NoControl(0.5f));
    }

    public void PickSpiritPart(SpiritPart part)
    {
        currentSpiritPoint += part.pointsHeld;
        spiritParts.Remove(part);
        Destroy(part.gameObject);
    }

    public IEnumerator NoControl(float time)
    {
        GameData.movementHandler.inControl = false;
        yield return new WaitForSeconds(time);
        GameData.movementHandler.inControl = true;
    }
}
