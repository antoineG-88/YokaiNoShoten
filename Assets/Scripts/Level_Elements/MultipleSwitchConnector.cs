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
                if(singleSwitch.IsON())
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
                if (!singleSwitch.IsON())
                {
                    isOn = false;
                }
            }
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }
}
