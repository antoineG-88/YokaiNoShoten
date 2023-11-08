using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerHPEvent : EventPart
{
    public int hpAmount;
    public override void StartEventPart()
    {
        base.StartEventPart();
        GameData.playerManager.SetHP(hpAmount);
        EndEventPart();
    }
}
