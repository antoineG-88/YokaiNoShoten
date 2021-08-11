using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvocationZone : MonoBehaviour
{
    public List<Enemy> connectedEnnemies;
    public bool provokeAllWhenOneIsprovoked;

    private bool shouldProvokeAll;
    private bool[] previousProvocation;

    private void Start()
    {
        previousProvocation = new bool[connectedEnnemies.Count];
        for (int i = 0; i < connectedEnnemies.Count; i++)
        {
            previousProvocation[i] = false;
        }
    }

    private void Update()
    {
        if(provokeAllWhenOneIsprovoked)
        {
            shouldProvokeAll = false;
            for (int i = 0; i < connectedEnnemies.Count; i++)
            {
                if(connectedEnnemies[i].provoked && !previousProvocation[i])
                {
                    previousProvocation[i] = true;
                    shouldProvokeAll = true;
                }
                if (!connectedEnnemies[i].provoked && previousProvocation[i])
                {
                    previousProvocation[i] = false;
                }
            }

            if(shouldProvokeAll)
            {
                ProvokeAllGroup();
            }
        }
    }


    private void ProvokeAllGroup()
    {
        for (int i = 0; i < connectedEnnemies.Count; i++)
        {
            connectedEnnemies[i].provoked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProvokeAllGroup();
    }
}
