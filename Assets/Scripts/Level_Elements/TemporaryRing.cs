using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryRing : Ring
{
    public float activeTime;
    public float inactiveTime;
    private float timer;
    SpriteRenderer sprite;
    private bool isActive;
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (timer < activeTime && isActive == true)
        {
            timer += Time.deltaTime;
            attachable = true;
            sprite.color = Color.green;
        }
        else if(timer > activeTime && isActive == true)
        {
            timer = 0;
            isActive = false;
        }
        

        if (timer < inactiveTime && isActive == false)
        {
            timer += Time.deltaTime;
            attachable = false;
            sprite.color = Color.red;
            
        }
        else if(timer > inactiveTime && isActive == false)
        {
            timer = 0;
            isActive = true;
        }
        
    }

    public override void AttachReaction()
    {

    }
}
