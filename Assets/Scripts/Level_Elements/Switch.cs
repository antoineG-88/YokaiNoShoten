﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Switch : Piercable
{
    public bool inverseOutput;
    [HideInInspector] public bool isOn;


    public virtual void Start()
    {

    }

    public void SwitchOnOff()
    {
        isOn = !isOn;
    }

    public bool IsON()
    {
        return inverseOutput ? !isOn : isOn;
    }
}
