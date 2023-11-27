using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFade : EventPart
{
    //public Color screenColor; 
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public float minSceenTime;
    [Header("Optionnal")]
    public Sound soundToPlayDuringBlackScreen;
    public List<GameObject> objectToDisable;
    public List<GameObject> objectToEnable;
    public Vector2 seikiNewPos;
    public CameraConstraintZone cameraChangeDuringScreen;
    public AudioSource source;

    private float fadeTimeElapsed;
    private float screenTimeElasped;
    private bool isScreenActive;
    private bool blackScreenStartFlag;
    private bool isUnfading;

    private void Update()
    {
        if (eventStarted && !eventEnded)
        {
            if (isScreenActive)
            {
                if (blackScreenStartFlag)
                {
                    blackScreenStartFlag = false;
                    for (int i = 0; i < objectToDisable.Count; i++)
                    {
                        objectToDisable[i].SetActive(false);
                    }

                    for (int i = 0; i < objectToEnable.Count; i++)
                    {
                        objectToEnable[i].SetActive(true);
                    }

                    if(seikiNewPos != Vector2.zero)
                    {
                        GameData.player.transform.position = seikiNewPos;
                        GameData.movementHandler.Propel(Vector2.zero, true);
                    }

                    if(cameraChangeDuringScreen != null)
                    {
                        eventTrigger.previousCamera = cameraConstraintDuringEventPart;
                        cameraConstraintDuringEventPart = cameraChangeDuringScreen;
                        eventTrigger.SetNewCameraConstraint();
                    }

                    if (soundToPlayDuringBlackScreen.clip != null)
                    {
                        source.PlayOneShot(soundToPlayDuringBlackScreen.clip, soundToPlayDuringBlackScreen.volumeScale);
                    }
                }

                if (screenTimeElasped < minSceenTime)
                {
                    screenTimeElasped += Time.deltaTime;
                }
                else
                {
                    fadeTimeElapsed = 0;
                    isUnfading = true;
                    isScreenActive = false;
                }
            }
            else
            {
                if (!isUnfading)
                {
                    if (fadeTimeElapsed < fadeInTime)
                    {
                        BlackScreenManager.SetAlpha(fadeTimeElapsed / fadeInTime);
                        fadeTimeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        BlackScreenManager.SetAlpha(1);
                        screenTimeElasped = 0;
                        isScreenActive = true;
                    }
                }
                else
                {
                    if (fadeTimeElapsed < fadeOutTime)
                    {
                        BlackScreenManager.SetAlpha(1 - (fadeTimeElapsed / fadeOutTime));
                        fadeTimeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        BlackScreenManager.SetAlpha(0);
                        EndEventPart();
                    }
                }
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();

        BlackScreenManager.SetAlpha(0);
        if(fadeInTime == 0 && fadeOutTime == 0 && minSceenTime == 0)
        {
            for (int i = 0; i < objectToDisable.Count; i++)
            {
                objectToDisable[i].SetActive(false);
            }

            for (int i = 0; i < objectToEnable.Count; i++)
            {
                objectToEnable[i].SetActive(true);
            }
            EndEventPart();
        }
        else
        {
            fadeTimeElapsed = 0;
            blackScreenStartFlag = true;
            isUnfading = false;
        }
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
        BlackScreenManager.SetAlpha(0);
    }
}
