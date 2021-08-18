using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceSwitch : Switch
{
    public Animator animator;

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        isOn = true;
        return false;
    }

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        animator.SetBool("LeverOn", isOn);
    }
}
