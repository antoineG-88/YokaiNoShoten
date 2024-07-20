using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Steamworks;

public class SteamIntegration : MonoBehaviour
{
    public static bool isInitialized;



    void Awake()
    {
        try
        {
            SteamClient.Init(2898970);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }

        isInitialized = SteamClient.IsValid;
        if (isInitialized)
        {
            //Debug.Log("Succesfuly connected to steam with account : " + SteamClient.Name);
        }
        else
        {
            Debug.LogWarning("Can't connect to steam");
        }
    }

    void Update()
    {
        SteamClient.RunCallbacks();
        SteamFriends.OnGameOverlayActivated += OnActivateOverlay;
    }


    void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }

    public static void UnlockAchievement(Achievement achievement)
    {
        if(isInitialized)
        {
            Steamworks.Data.Achievement steamAchievement = new Steamworks.Data.Achievement(achievement.ToString());

            steamAchievement.Trigger();
            //Debug.Log($"Achievement {steamAchievement.Name} unlocked");
        }
        else
        {
            Debug.LogWarning("Can't unlock achievement because init to steam failed");
        }
    }

    public static void ResetAllAchievements()
    {
        if (isInitialized)
        {
            SteamUserStats.ResetAll(true);
            //Debug.Log("All achievements reset");
        }
        else
        {
            Debug.LogWarning("Can't reset achievement because init to steam failed");
        }
    }

    public static bool IsAchievementUnlocked(Achievement achievement)
    {
        if (isInitialized)
        {
            Steamworks.Data.Achievement steamAchievement = new Steamworks.Data.Achievement(achievement.ToString());

            return steamAchievement.State;
        }
        else
        {
            return false;
        }
    }

    private void OnActivateOverlay(bool isActivated)
    {
        if(PauseManager.I != null)
            PauseManager.I.OnOverlayOpen(isActivated);

        if (MenuManager.I != null)
            MenuManager.I.OnOverlayOpen(isActivated);
    }
}
