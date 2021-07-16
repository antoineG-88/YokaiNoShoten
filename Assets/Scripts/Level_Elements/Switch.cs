using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Switch : Piercable
{
    public bool startOnON;
    [HideInInspector] public bool isOn;

    public virtual void Start()
    {
        isOn = startOnON;
    }

    public void SwitchOnOff()
    {
        isOn = !isOn;
    }
}
