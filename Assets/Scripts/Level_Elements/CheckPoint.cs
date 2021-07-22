using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public float timeToRegenerate;
    [Header("Temporary")]
    public float regenFBStartSize;
    public float regenFBMaxSize;
    public GameObject regenerationFeedback;
    public GameObject checkPointActivationDisplay;

    private bool isPlayerInRange;
    private bool isActivated;
    private float elapsedTimeNearCheckPoint;
    private bool saveFlag;
    private bool regenerateFlag;

    void Start()
    {
        GameManager.allZoneCheckPoints.Add(this);
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
                isActivated = true;
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
