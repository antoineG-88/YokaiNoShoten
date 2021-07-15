using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectSwitch : Switch
{
    private bool isInRange;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
        }
    }
    private void Update()
    {
        if (isInRange && Input.GetButtonDown("YButton"))
        {
            SwitchOnOff();
        }
    }
}
