using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IllustrationEvent : EventPart
{
    public Sprite[] illustrations;
    public float[] illustrationTimes;
    public AudioClip cinematicSoundEffects;
    public AudioClip cinematicMusic;
    public Image image;

    private int currentIllustrationStep;
    private float currentIlluTimeSpend;
    private bool isInCinematic;

    private void Update()
    {
       if(isInCinematic)
        {
            if(currentIlluTimeSpend > illustrationTimes[currentIllustrationStep])
            {
                currentIllustrationStep++;
                if(currentIllustrationStep < illustrations.Length)
                {
                    image.sprite = illustrations[currentIllustrationStep];
                    currentIlluTimeSpend = 0;
                }
                else
                {
                    EndEventPart();
                }
            }
            else
            {
                currentIlluTimeSpend += Time.deltaTime;
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        currentIllustrationStep = 0;
        currentIlluTimeSpend = 0;
        image.gameObject.SetActive(true);
        image.sprite = illustrations[0];
        isInCinematic = true;
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
        image.gameObject.SetActive(false);
        isInCinematic = false;
    }
}
