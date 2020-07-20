using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [HideInInspector]
    public bool attachable;

    void Start()
    {
        attachable = true;
    }

}
