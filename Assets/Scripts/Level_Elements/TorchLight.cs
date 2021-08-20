using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : Switch
{
    [HideInInspector] public TorchSystem torchSystem;
    [HideInInspector] public bool isLit;
    private Animator animator;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        isOn = isLit;
        if(isLit && !torchSystem.isTorchGrabbed && !torchSystem.isOn)
        {
            isLit = false;
            animator.SetBool("isLit", false);
        }
        if(torchSystem.isOn)
        {
            Lit();
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }

    public void Lit()
    {
        isLit = true;
        animator.SetBool("isLit", true);
    }
}
