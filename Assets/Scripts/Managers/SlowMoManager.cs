using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowMoManager : MonoBehaviour
{
    [Range(0.00001f, 1f)] public float slowMoInitialRatio;
    public float smoothTime;
    [Header("Slow Mo Effect settings")]
    public VolumeProfile baseProfile;
    public VolumeProfile slowMoStartProfile;
    public Volume volume;
    public float slowMoEffectLerpRatio;

    [HideInInspector] public bool inSlowMo;
    private Coroutine currentStopCoroutine;
    private Coroutine currentStartCoroutine;
    private float slowMoEffectCurrentState;
    private bool slowMoEffectActive;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;
    private float aberrationBaseIntensity;
    private float distortionBaseIntensity;
    private float vignetteBaseIntensity;
    private VolumeProfile slowMoProfile;
    private float lastRealTime;


    void Start()
    {
        inSlowMo = false;
        slowMoEffectCurrentState = 0;
        slowMoEffectActive = false;
        slowMoProfile = Instantiate(slowMoStartProfile);

        slowMoProfile.TryGet<ChromaticAberration>(out chromaticAberration);
        slowMoProfile.TryGet<LensDistortion>(out lensDistortion);
        slowMoProfile.TryGet<Vignette>(out vignette);

        chromaticAberration = Instantiate(chromaticAberration);
        lensDistortion = Instantiate(lensDistortion);
        vignette = Instantiate(vignette);

        for (int i = 0; i < slowMoProfile.components.Count; i++)
        {
            switch (slowMoProfile.components[i].name)
            {
                case "ChromaticAberration":
                    slowMoProfile.components[i] = chromaticAberration;
                    break;

                case "Vignette":
                    slowMoProfile.components[i] = vignette;
                    break;

                case "LensDistortion":
                    slowMoProfile.components[i] = lensDistortion;
                    break;
            }
        }

        chromaticAberration.intensity.overrideState = true;
        vignette.intensity.overrideState = true;
        lensDistortion.intensity.overrideState = true;

        aberrationBaseIntensity = chromaticAberration.intensity.value;
        distortionBaseIntensity = lensDistortion.intensity.value;
        vignetteBaseIntensity = vignette.intensity.value;
    }

    private void Update()
    {

        if(slowMoEffectActive)
        {
            slowMoEffectCurrentState = Mathf.Lerp(slowMoEffectCurrentState, 1, slowMoEffectLerpRatio * (Time.realtimeSinceStartup - lastRealTime));

        }
        else
        {
            slowMoEffectCurrentState = Mathf.Lerp(slowMoEffectCurrentState, 0, slowMoEffectLerpRatio * (Time.realtimeSinceStartup - lastRealTime));
            if (slowMoEffectCurrentState < 0.01f)
            {
                volume.sharedProfile = baseProfile;
            }
        }

        chromaticAberration.intensity.value = Mathf.Lerp(0, aberrationBaseIntensity, slowMoEffectCurrentState);
        vignette.intensity.value = Mathf.Lerp(0, vignetteBaseIntensity, slowMoEffectCurrentState);
        lensDistortion.intensity.value = Mathf.Lerp(0, distortionBaseIntensity, slowMoEffectCurrentState);


        lastRealTime = Time.realtimeSinceStartup;
    }

    private void StartSlowMoEffect()
    {
        slowMoEffectActive = true;
        volume.sharedProfile = slowMoProfile;
    }

    private void StopSlowMoEffect()
    {
        slowMoEffectActive = false;
    }

    public void StartSlowMo(float ratioMultiplier)
    {
        Time.timeScale = slowMoInitialRatio * ratioMultiplier;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        inSlowMo = true;
        StartSlowMoEffect();
    }

    public void StopSlowMo()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        inSlowMo = false;
        StopSlowMoEffect();
    }

    public void StartSmoothSlowMo(float ratioMultiplier, float startDelay)
    {
        currentStartCoroutine = StartCoroutine(CSmoothStartSlowMo(ratioMultiplier, startDelay));
    }

    public void StopSmoothSlowMo(float stopDelay)
    {
        currentStopCoroutine = StartCoroutine(CSmoothStopSlowMo(stopDelay));
    }

    private IEnumerator CSmoothStartSlowMo(float ratioMultiplier, float startDelay)
    {
        StartSlowMoEffect();

        if (startDelay > 0)
            yield return new WaitForSecondsRealtime(startDelay);


        if (currentStopCoroutine != null)
        {
            StopCoroutine(currentStopCoroutine);
        }

        float timer = 0;
        while(timer < smoothTime)
        {
            Time.timeScale = Mathf.Lerp(1, slowMoInitialRatio * ratioMultiplier, timer / smoothTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            inSlowMo = true;

            timer += Time.deltaTime * (1 / Time.timeScale);
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = slowMoInitialRatio * ratioMultiplier;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        inSlowMo = true;
    }

    private IEnumerator CSmoothStopSlowMo(float stopDelay)
    {
        if (stopDelay > 0)
            yield return new WaitForSecondsRealtime(stopDelay);

        StopSlowMoEffect();

        if (currentStartCoroutine != null)
        {
            StopCoroutine(currentStartCoroutine);
        }
        float timer = 0;
        float startSlowMoRatio = Time.timeScale;
        while (timer < smoothTime)
        {
            Time.timeScale = Mathf.Lerp(startSlowMoRatio, 1, timer / smoothTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            inSlowMo = true;

            timer += Time.deltaTime * (1 / Time.timeScale);
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        inSlowMo = false;
    }
}
