﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFade : EventPart
{
    public Color screenColor; 
    public float fadeTime;
    public float minSceenTime;
    [Header("Optionnal")]
    //public CameraConstraintZone cameraConstraintToApplyUntilUnfade;
    public AudioClip soundToPlayDuringBlackScreen;
    public List<GameObject> objectToDisable;
    public List<GameObject> objectToEnable;


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
                        BlackScreenManager.blackScreen.color = Color.Lerp(Color.clear, screenColor, fadeTimeElapsed / fadeTime);
                        fadeTimeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        BlackScreenManager.blackScreen.color = screenColor;
                        screenTimeElasped = 0;
                        isScreenActive = true;
                    }
                }
                else
                {
                    if (fadeTimeElapsed < fadeTime)
                    {
                        BlackScreenManager.blackScreen.color = Color.Lerp(screenColor, Color.clear, fadeTimeElapsed / fadeTime);
                        fadeTimeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        BlackScreenManager.blackScreen.color = Color.clear;
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

        BlackScreenManager.blackScreen.color = Color.clear;
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
        BlackScreenManager.blackScreen.color = Color.clear;
    }
}
