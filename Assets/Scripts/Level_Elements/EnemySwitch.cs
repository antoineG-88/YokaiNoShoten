using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwitch : Switch
{
    public List<Enemy> enemyToDestroy;

    private void Update()
    {
        if(!isOn)
        {
            UpdateState();
        }
    }

    private void UpdateState()
    {
        isOn = true;
        foreach (Enemy enemy in enemyToDestroy)
        {
            if (enemy != null && !enemy.isDying)
            {
                isOn = false;
            }
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }
}
