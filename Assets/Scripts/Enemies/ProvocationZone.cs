using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvocationZone : MonoBehaviour
{
    public List<Enemy> connectedEnnemies;
    public bool provokeAllWhenOneIsprovoked;
    public bool enemiesCanLoseAggro;

    private bool shouldProvokeAll;
    private bool[] previousProvocation;
    private bool hasBeenTriggered;
    private bool enemiesDied;

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
        if(provokeAllWhenOneIsprovoked && !enemiesDied)
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

        if(hasBeenTriggered && !enemiesCanLoseAggro && !enemiesDied)
        {
            ProvokeAllGroup();
        }
    }


    private void ProvokeAllGroup()
    {
        for (int i = 0; i < connectedEnnemies.Count; i++)
        {
            enemiesDied = true;
            if(connectedEnnemies[i] != null)
            {
                connectedEnnemies[i].provoked = true;
                enemiesDied = false;
            }
        }
        hasBeenTriggered = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!enemiesDied)
            ProvokeAllGroup();
    }
}
