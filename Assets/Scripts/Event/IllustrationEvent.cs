using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IllustrationEvent : EventPart
{
    public Sprite[] illustrations;
    public float[] illustrationTimes;
    public string[] illuDescriptions;
    public float eventFadeInTime = 0.5f;
    public float eventFadeOutTime = 0.5f;
    public float illustrationTransitionFadeTime = 0.5f;
    public Image illustrationImage;
    public Image backgroundImage;
    public Text descriptionText;
    [Space]
    public AudioSource cinematicSource;
    public Sound[] cinematicSoundEffects;
    public bool startNextEventPartWhenStartUnfade;

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
        //illustrationImage.gameObject.SetActive(false);
        //backgroundImage.gameObject.SetActive(false);
        isInCinematic = false;
    }
    public IEnumerator FadeEventIn()
    {
        float timer = 0;
        backgroundImage.gameObject.SetActive(true);
        backgroundImage.CrossFadeAlpha(0, 0, true);
        backgroundImage.CrossFadeAlpha(1, eventFadeInTime, true);

        while (timer < eventFadeInTime)
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

        if(illuDescriptions.Length > 0)
        {
            descriptionText.gameObject.SetActive(true);
            descriptionText.CrossFadeAlpha(0, 0, true);
            descriptionText.text = illuDescriptions[currentIllustrationStep];
            descriptionText.CrossFadeAlpha(1, illustrationTransitionFadeTime, true);
        }

        if (cinematicSoundEffects.Length > 0 && cinematicSoundEffects[currentIllustrationStep].clip != null)
            cinematicSource.PlayOneShot(cinematicSoundEffects[currentIllustrationStep].clip, cinematicSoundEffects[currentIllustrationStep].volumeScale);

        yield return new WaitForSeconds(illustrationTimes[currentIllustrationStep]);


        StartCoroutine(FadeOutIllustration());
    }

    public IEnumerator FadeOutIllustration()
    {
        illustrationImage.CrossFadeAlpha(0, illustrationTransitionFadeTime, true);
        if (illuDescriptions.Length > 0)
            descriptionText.CrossFadeAlpha(0, illustrationTransitionFadeTime, true);

        yield return new WaitForSeconds(illustrationTransitionFadeTime);

        currentIllustrationStep++;
        if (currentIllustrationStep < illustrations.Length)
        {
            illustrationImage.sprite = illustrations[currentIllustrationStep];
            if (illuDescriptions.Length > 0)
                descriptionText.text = illuDescriptions[currentIllustrationStep];

            yield return new WaitForSeconds(0.3f);

            StartCoroutine(ShowNextIllustration());
        }
        else
        {
            EndEventPart();
            StartCoroutine(FadeEventOut());
        }
    }

    public IEnumerator FadeEventOut()
    {
        float timer = 0;
        backgroundImage.gameObject.SetActive(true);
        backgroundImage.CrossFadeAlpha(1, 0, true);
        backgroundImage.CrossFadeAlpha(0, eventFadeOutTime, true);

        while (timer < eventFadeOutTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        illustrationImage.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
    }
}
