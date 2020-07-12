using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage, Vector2 knockBackDirectedForce)
    {
        GameData.playerVisuals.isHurt = 5;
        GameData.movementHandler.Propel(knockBackDirectedForce, false);
        StartCoroutine(NoControl(0.5f));
    }

    public IEnumerator NoControl(float time)
    {
        GameData.movementHandler.inControl = false;
        yield return new WaitForSeconds(time);
        GameData.movementHandler.inControl = true;
    }
}
