using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piercable : MonoBehaviour
{
    private int startingLayer;
    protected bool doNotReableCollider;
    protected bool intargetable;

    private void Awake()
    {
        startingLayer = gameObject.layer;
    }

    /// <summary>
    /// Trigger the effect of the pierce attack
    /// </summary>
    /// <param name="damage">the damage dealt to the target if it can</param>
    /// <param name="directedForce">the force to apply if target can be moved</param>
    /// <returns>Returns true if the target is protected and cannot be pierced</returns>
    public abstract bool PierceEffect(int damage, Vector2 directedForce);

    public IEnumerator DisablePiercable()
    {
        gameObject.layer = GameData.noPiercableLayer;
        intargetable = true;
        if(!doNotReableCollider)
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.layer = startingLayer;
            intargetable = false;
        }
    }
}
