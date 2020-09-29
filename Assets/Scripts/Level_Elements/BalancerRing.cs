using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancerRing : Ring
{
    private Balancer balancer;

    new void Start()
    {
        base.Start();
        balancer = transform.parent.GetComponent<Balancer>();
        attachable = true;
    }

    public override void AttachReaction()
    {
        balancer.StopBalancing();
    }
}
