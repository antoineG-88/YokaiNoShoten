using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementEvent : EventPart
{
    public Achievement achievementToTrigger;

    public override void StartEventPart()
    {
        base.StartEventPart();
        SteamIntegration.UnlockAchievement(achievementToTrigger);
        EndEventPart();
    }
}

public enum Achievement { ACH_FADE_WITNESS, ACH_DESTROY_CORE, ACH_ZERO_DEATH, ACH_MEET_RESISTANCE, ACH_LIBERATOR, ACH_REACH_CORE, ACH_AIRBORNE, ACH_PIERCER, ACH_NO_REST };
