using UnityEngine;
using System.Collections;

public class SlowMoManager : MonoBehaviour
{
    [Range(0.00001f, 1f)] public float slowMoInitialRatio;
    public float smoothTime;

    [HideInInspector] public bool inSlowMo;

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

    public IEnumerator SmoothStartSlowMo(float ratioMultiplier, float startDelay)
    {
        yield return new WaitForSecondsRealtime(startDelay);
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

    public IEnumerator SmoothStopSlowMo(float stopDelay)
    {
        if (stopDelay > 0)
            yield return new WaitForSecondsRealtime(stopDelay);
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
