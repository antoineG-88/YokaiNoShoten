using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : Switch
{
    public ParticleSystem activatedParticle;
    [HideInInspector] public TorchSystem torchSystem;
    [HideInInspector] public bool isLit;
    private Animator animator;
    public AudioSource source;
    public AudioClip litSound;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        isOn = isLit;

        if(torchSystem.isOn)
        {
            isLit = true;
        }

        animator.SetBool("isLit", isLit);
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }

    public void Lit()
    {
        isLit = true;
        activatedParticle.Play();
        source.PlayOneShot(litSound);
    }

    public void UnLit()
    {
        isLit = false;
        activatedParticle.Stop();
    }
}
