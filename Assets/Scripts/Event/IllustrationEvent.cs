using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IllustrationEvent : EventPart
{
    public Sprite[] illustrations;
    public float[] illustrationTimes;
    public float eventFadeTime = 0.5f;
    public float illustrationTransitionFadeTime = 0.5f;
    public Image illustrationImage;
    public Image backgroundImage;
    [Space]
    public AudioSource cinematicSource;
    public Sound[] cinematicSoundEffects;

    private int currentIllustrationStep;
    private bool isInCinematic;

    private void Update()
    {
        if (isInCinematic)
        {

        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        currentIllustrationStep = 0;
        StartCoroutine(FadeEventIn());
        isInCinematic = true;
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
        illustrationImage.gameObject.SetActive(false);
        isInCinematic = false;
    }
    public IEnumerator FadeEventIn()
    {
        float timer = 0;
        backgroundImage.gameObject.SetActive(true);
        backgroundImage.CrossFadeAlpha(0, 0, true);
        backgroundImage.CrossFadeAlpha(1, eventFadeTime, true);

        while (timer < eventFadeTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ShowNextIllustration());
    }

    public IEnumerator ShowNextIllustration()
    {
        illustrationImage.gameObject.SetActive(true);
        illustrationImage.CrossFadeAlpha(0, 0, true);
        illustrationImage.sprite = illustrations[currentIllustrationStep];
        illustrationImage.CrossFadeAlpha(1, illustrationTransitionFadeTime, true);

        if(cinematicSoundEffects[currentIllustrationStep].clip != null)
            cinematicSource.PlayOneShot(cinematicSoundEffects[currentIllustrationStep].clip, cinematicSoundEffects[currentIllustrationStep].volumeScale);

        yield return new WaitForSeconds(illustrationTimes[currentIllustrationStep]);


        StartCoroutine(FadeOutIllustration());
    }
    public IEnumerator FadeOutIllustration()
    {
        illustrationImage.CrossFadeAlpha(0, illustrationTransitionFadeTime, true);

        yield return new WaitForSeconds(illustrationTransitionFadeTime);

        currentIllustrationStep++;
        if (currentIllustrationStep < illustrations.Length)
        {
            illustrationImage.sprite = illustrations[currentIllustrationStep];

            yield return new WaitForSeconds(0.3f);

            StartCoroutine(ShowNextIllustration());
        }
        else
        {
            EndEventPart();
        }
    }
}
