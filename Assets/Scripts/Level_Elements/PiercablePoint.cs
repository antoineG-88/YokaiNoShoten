﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercablePoint : Piercable
{
    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }
}