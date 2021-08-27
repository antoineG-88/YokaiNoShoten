using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentTail : Piercable
{
    public Serpent serpent;

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        triggerSlowMo = true;
        serpent.DisableSpikes();
        return false;
    }
}
