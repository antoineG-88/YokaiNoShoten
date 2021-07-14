using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool startOnON;
    [HideInInspector] public bool isOn;

    void Start()
    {
        isOn = startOnON;
    }

    public void SwitchOnOff()
    {
        isOn = !isOn;
    }
}
