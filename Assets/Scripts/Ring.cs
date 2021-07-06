using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ring : MonoBehaviour
{
    [HideInInspector]
    public bool attachable;

    public void Start()
    {
        attachable = true;
    }

    public abstract void AttachReaction();
}
