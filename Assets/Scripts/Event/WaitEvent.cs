using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitEvent : EventPart
{
    public float waitTime;
    public bool unscaledTime;

    public override void StartEventPart()
    {
        base.StartEventPart();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        if(unscaledTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }
        EndEventPart();
    }
}
