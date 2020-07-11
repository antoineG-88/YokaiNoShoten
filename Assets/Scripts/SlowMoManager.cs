using UnityEngine;

public class SlowMoManager : MonoBehaviour
{
    [Range(0.00001f, 1f)] public float slowMoInitialRatio;

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
}
