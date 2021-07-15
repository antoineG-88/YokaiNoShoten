using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentTail : Piercable
{
    public Serpent serpent;

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        serpent.DisableSpikes();
        return false;
    }
}
