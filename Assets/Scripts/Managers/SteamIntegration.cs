using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamIntegration : MonoBehaviour
{

    void Awake()
    {
        try
        {
            SteamClient.Init(2898970);
            //Debug.Log("Succesfuly connected to steam with account : " + SteamClient.Name);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    void Update()
    {
        SteamClient.RunCallbacks();
    }


    void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }

    public static void UnlockAchievement(Achievement achievement)
    {
        Steamworks.Data.Achievement steamAchievement = new Steamworks.Data.Achievement(achievement.ToString());

        steamAchievement.Trigger();
        //Debug.Log($"Achievement {steamAchievement.Name} unlocked");
    }

    public static void ResetAllAchievements()
    {
        SteamUserStats.ResetAll(true);
        //Debug.Log("All achievements reset");
    }

    public static bool IsAchievementUnlocked(Achievement achievement)
    {
        Steamworks.Data.Achievement steamAchievement = new Steamworks.Data.Achievement(achievement.ToString());

        return steamAchievement.State;
    }
}
