using UnityEngine;
using System.Collections;

public class SlowMoManager : MonoBehaviour
{
    [Range(0.00001f, 1f)] public float slowMoInitialRatio;
    public float smoothTime;

    [HideInInspector] public bool inSlowMo;
    private Coroutine currentStopCoroutine;
    private Coroutine currentStartCoroutine;
    void Start()
    {
        inSlowMo = false;
    }

    public void StartSlowMo(float ratioMultiplier)
    {
        Time.timeScale = slowMoInitialRatio * ratioMultiplier;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        inSlowMo = true;
    }

    public void StopSlowMo()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        inSlowMo = false;
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
