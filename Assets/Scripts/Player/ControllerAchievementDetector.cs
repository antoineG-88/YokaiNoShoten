using System.Collections.Generic;
using UnityEngine;

public class ControllerAchievementDetector : MonoBehaviour
{
    private bool hasAchievedAirborne;
    private bool hasAchievedSpiritBullet;

    private List<int> checkpointReachedSinceLastGrounded;
    private int currentConsecutivePierceThroughEnemies;

    void Start()
    {
        if(SteamIntegration.IsAchievementUnlocked(Achievement.ACH_AIRBORNE))
        {
            hasAchievedAirborne = true;
        }
        else
        {
            checkpointReachedSinceLastGrounded = new List<int>();
        }

        if (SteamIntegration.IsAchievementUnlocked(Achievement.ACH_PIERCER))
        {
            hasAchievedSpiritBullet = true;
        }
    }

    void Update()
    {
        if (!hasAchievedAirborne)
        {
            CheckAirborne();
        }

        if (!hasAchievedSpiritBullet)
        {
            CheckSpiritBullet();
        }
    }

    private void CheckAirborne()
    {
        if(GameData.movementHandler.isGrounded && checkpointReachedSinceLastGrounded.Count > 0)
        {
            //Debug.Log("Reset airborne");
            checkpointReachedSinceLastGrounded.Clear();
        }
        else
        {
            if (checkpointReachedSinceLastGrounded.Count >= 3)
            {
                SteamIntegration.UnlockAchievement(Achievement.ACH_AIRBORNE);
                hasAchievedAirborne = true;
            }
        }
    }

    public void IncrementNewCheckpointReached(int checkpointID)
    {
        if(!hasAchievedAirborne)
        {
            if (!checkpointReachedSinceLastGrounded.Contains(checkpointID))
            {
                checkpointReachedSinceLastGrounded.Add(checkpointID);
                //Debug.Log("New checkpoint reached since last grounded : " + checkpointID);
            }
        }
    }

    private void CheckSpiritBullet()
    {
        if(!GameData.pierceHandler.isPhasing)
        {
            if(currentConsecutivePierceThroughEnemies > 0)
            {
                //Debug.Log("Reset pierce consecutive count");
                currentConsecutivePierceThroughEnemies = 0;
            }
        }

        if (currentConsecutivePierceThroughEnemies >= 5)
        {
            SteamIntegration.UnlockAchievement(Achievement.ACH_PIERCER);
            hasAchievedSpiritBullet = true;
        }
    }

    public void IncrementEnemyPierce()
    {
        if (!hasAchievedSpiritBullet)
        {
            currentConsecutivePierceThroughEnemies++;
            //Debug.Log("Incr enemy cons : " + currentConsecutivePierceThroughEnemies);
        }
    }
}
