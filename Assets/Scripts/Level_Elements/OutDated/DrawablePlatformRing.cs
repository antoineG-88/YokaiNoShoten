using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawablePlatformRing : Ring
{
    public bool isRightRing;
    [HideInInspector] public bool moveRight;
    [HideInInspector] public bool moveLeft;

    public new void Start()
    {
        base.Start();
        moveLeft = false;
        moveRight = false;
    }
    private void Update()
    {
        
    }
    public override void AttachReaction()
    {
        if(isRightRing == true)
        {
            moveRight = true;
            moveLeft = false;
        }
        else
        {
            moveLeft = true;
            moveRight = false;
            
        }
    }
}
