using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piercable : MonoBehaviour
{
    /// <summary>
    /// Trigger the effect of the pierce attack
    /// </summary>
    /// <param name="damage">the damage dealt to the target if it can</param>
    /// <param name="directedForce">the force to apply if target can be moved</param>
    /// <returns>Returns true if the target is protected and cannot be pierced</returns>
    public abstract bool PierceEffect(int damage, Vector2 directedForce);
}
