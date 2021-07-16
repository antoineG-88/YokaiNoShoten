using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : Switch
{
    public float maxLitTime;

    [HideInInspector] public TorchSystem torchSystem;
    [HideInInspector] public bool isLit;
    private Animator animator;
    private float timeLitElapsed;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        isOn = isLit;
        if(isLit && ((!torchSystem.isOn && torchSystem.stayOn) || !torchSystem.stayOn))
        {
            if(timeLitElapsed > maxLitTime)
            {
                isLit = false;
                animator.SetBool("isLit", false);
            }
            timeLitElapsed += Time.deltaTime;
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }

    public void Lit()
    {
        isLit = true;
        timeLitElapsed = 0;
        animator.SetBool("isLit", true);
    }
}
