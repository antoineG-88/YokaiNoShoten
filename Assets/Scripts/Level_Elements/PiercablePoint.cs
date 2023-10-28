using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercablePoint : Piercable
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        triggerSlowMo = true;
        animator.SetTrigger("Pierce");
        return false;
    }
}
