using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceSwitch : Switch
{
    public bool pierceTriggerSlowMo;
    public Animator animator;

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        triggerSlowMo = pierceTriggerSlowMo;
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
