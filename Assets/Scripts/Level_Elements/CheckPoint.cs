using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPointNumber;
    public float timeToRegenerate;
    [Header("Temporary")]
    public float regenFBStartSize;
    public float regenFBMaxSize;
    public GameObject regenerationFeedback;
    public GameObject checkPointActivationDisplay;

    private bool isPlayerInRange;
    [HideInInspector] public bool isActivated;
    private float elapsedTimeNearCheckPoint;
    private bool saveFlag;
    private bool regenerateFlag;

    private void Start()
    {
        LevelManager.allZoneCheckPoints.Add(this);
    }

    void Update()
    {
        UpdateBehavior();
    }

    private void UpdateBehavior()
    {
        if(isPlayerInRange)
        {
            elapsedTimeNearCheckPoint += Time.deltaTime;
            regenerationFeedback.SetActive(true);

            if (elapsedTimeNearCheckPoint > timeToRegenerate && regenerateFlag)
            {
                regenerationFeedback.transform.localScale = Vector2.one * regenFBMaxSize;
                regenerateFlag = false;
                Regenerate();
            }
            else
            {
                regenerationFeedback.transform.localScale = Vector2.one * Mathf.Lerp(regenFBStartSize, regenFBMaxSize, elapsedTimeNearCheckPoint / timeToRegenerate);
            }

            if(saveFlag)
            {
                saveFlag = false;
                SaveAsCurrentCheckPoint();
            }
        }
        else
        {
            elapsedTimeNearCheckPoint = 0;
            regenerationFeedback.SetActive(false);
            regenerateFlag = true;
            saveFlag = true;
        }

        checkPointActivationDisplay.SetActive(isActivated);
    }

    private void SaveAsCurrentCheckPoint()
    {
        LevelManager.ActivateSingleCheckPoint(this);
        GameManager.SaveProgression(this);
    }

    private void Regenerate()
    {
        GameData.playerManager.Heal(10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
