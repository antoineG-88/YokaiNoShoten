using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPointNumber;
    public float timeToRegenerate;
    public Animator animator;
    public Sound activationSound;

    private bool isPlayerInRange;
    [HideInInspector] public bool isActivated;
    private float elapsedTimeNearCheckPoint;
    private bool saveFlag;
    private bool regenerateFlag;
    private AudioSource source;

    private void Start()
    {
        LevelManager.allZoneCheckPoints.Add(this);

        source = GetComponent<AudioSource>();
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

            if (elapsedTimeNearCheckPoint > timeToRegenerate && regenerateFlag)
            {
                regenerateFlag = false;
                Regenerate();

                if (saveFlag)
                {
                    saveFlag = false;
                    SaveAsCurrentCheckPoint();
                }
            }
        }
        else
        {
            elapsedTimeNearCheckPoint = 0;
            regenerateFlag = true;
            saveFlag = true;
        }
        if(animator.transform.parent.gameObject.activeSelf)
            animator.SetBool("Activated", isActivated);
    }

    private void SaveAsCurrentCheckPoint()
    {
        if(activationSound.clip != null && !isActivated)
            source.PlayOneShot(activationSound.clip, activationSound.volumeScale);
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
