using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFade : EventPart
{
    //public Color screenColor; 
    public float fadeTime;
    public float minSceenTime;
    [Header("Optionnal")]
    public AudioClip soundToPlayDuringBlackScreen;
    public List<GameObject> objectToDisable;
    public List<GameObject> objectToEnable;
    public Vector2 seikiNewPos;
    public CameraConstraintZone cameraChangeDuringScreen;

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

                    if (soundToPlayDuringBlackScreen != null)
                    {
                        //jouer le gros sooonnnnn
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
                    if (fadeTimeElapsed < fadeTime)
                    {
                        BlackScreenManager.SetAlpha(fadeTimeElapsed / fadeTime);
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
                    if (fadeTimeElapsed < fadeTime)
                    {
                        BlackScreenManager.SetAlpha(1 - (fadeTimeElapsed / fadeTime));
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
        fadeTimeElapsed = 0;
        blackScreenStartFlag = true;
        isUnfading = false;

        BlackScreenManager.SetAlpha(0);
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
        BlackScreenManager.SetAlpha(0);
    }
}
