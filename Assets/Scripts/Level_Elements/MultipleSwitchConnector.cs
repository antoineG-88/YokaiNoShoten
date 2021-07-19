using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSwitchConnector : Switch
{
    public List<Switch> switches;
    public bool isOr;

    private void Update()
    {
        if(isOr)
        {
            isOn = false;
            foreach (Switch singleSwitch in switches)
            {
                if(singleSwitch.isOn)
                {
                    isOn = true;
                }
            }
        }
        else
        {
            isOn = true;

            foreach (Switch singleSwitch in switches)
            {
                if (!singleSwitch.isOn)
                {
                    isOn = false;
                }
            }
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }
}
