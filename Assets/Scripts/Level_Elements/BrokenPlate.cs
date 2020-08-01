using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPlate : DashInteraction
{
    public GameObject ring;
    bool asAlreadyBroke;
    SpriteRenderer sprite;
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    public override void DashReaction()
    {
        if(asAlreadyBroke == false)
        {
            ring.SetActive(true);
            asAlreadyBroke = true;
            sprite.color = Color.black;
        } 
    }
}
