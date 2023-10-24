using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumblesManager : MonoBehaviour
{
    public static RumblesManager I;
    public static bool rumblesAreEnabled;

    public float motorsUpdateFrequence;
    [Header("Take damage")]
    public AnimationCurve takeDamageRumbleIntensityByTime;
    public AnimationCurve takeDamageHighRumbleIntensityByTime;
    [Header("Death")]
    public AnimationCurve deathLowRumbleIntensityByTime;
    public AnimationCurve deathHighRumbleIntensityByTime;
    [Header("Grapple")]
    public AnimationCurve grappleLowRumbleIntensityByTime;
    public AnimationCurve grappleHighRumbleIntensityByTime;
    [Header("Dash")]
    public AnimationCurve dashLowRumbleIntensityByTime;
    public AnimationCurve dashHighRumbleIntensityByTime;
    [Header("Phasing")]
    public AnimationCurve pierceStartLowRumbleIntensityByTime;
    public AnimationCurve pierceStartHighRumbleIntensityByTime;
    public AnimationCurve pierceEndLowRumbleIntensityByTime;
    public AnimationCurve pierceEndHighRumbleIntensityByTime;
    [Space]
    public Gamepad gamepad;
    [HideInInspector] public Coroutine currentRamble;
    private float timer;
    private float intensity;
    private float intensity2;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("rumbles"))
        {
            rumblesAreEnabled = PlayerPrefs.GetInt("rumbles") == 1 ? true : false;
        }
        else
        {
            EnableRumbles(true);
        }
    }

    private void Start()
    {
        gamepad = Gamepad.current;
    }

    public static void StartDashRumble()
    {
        if(I.currentRamble != null)
            I.StopCoroutine(I.currentRamble);
        if (rumblesAreEnabled)
            I.currentRamble = I.StartCoroutine(I.DashRumble());
    }

    public static void StartTakeDamageRumble()
    {
        if (I.currentRamble != null)
            I.StopCoroutine(I.currentRamble);
        if(rumblesAreEnabled)
            I.currentRamble = I.StartCoroutine(I.TakeDamageRumble());
    }

    public static void StartPhasingRumble()
    {
        if (I.currentRamble != null)
            I.StopCoroutine(I.currentRamble);
        if (rumblesAreEnabled)
            I.currentRamble = I.StartCoroutine(I.PierceStartRumble());
    }

    public static void EndPhasingRumble()
    {
        if (I.currentRamble != null)
            I.StopCoroutine(I.currentRamble);
        if (rumblesAreEnabled)
            I.currentRamble = I.StartCoroutine(I.PierceEndRumble());
    }

    public static void StartGrappleRumble()
    {
        if (I.currentRamble != null)
            I.StopCoroutine(I.currentRamble);
        if (rumblesAreEnabled)
            I.currentRamble = I.StartCoroutine(I.GrappleRumble());
    }

    public static void StartDeathRumble()
    {
        if (I.currentRamble != null)
            I.StopCoroutine(I.currentRamble);
        if (rumblesAreEnabled)
            I.currentRamble = I.StartCoroutine(I.DeathRumble());
    }

    private IEnumerator DashRumble()
    {
        timer = 0;
        intensity = dashLowRumbleIntensityByTime.Evaluate(timer);
        intensity2 = dashHighRumbleIntensityByTime.Evaluate(timer);
        while (timer < dashLowRumbleIntensityByTime.keys[dashLowRumbleIntensityByTime.length - 1].time)
        {
            intensity = dashLowRumbleIntensityByTime.Evaluate(timer);
            intensity2 = dashHighRumbleIntensityByTime.Evaluate(timer);
            gamepad.SetMotorSpeeds(intensity, intensity2);
            timer += motorsUpdateFrequence;
            yield return new WaitForSecondsRealtime(motorsUpdateFrequence);
        }
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

    private IEnumerator PierceStartRumble()
    {
        timer = 0;
        intensity = pierceStartLowRumbleIntensityByTime.Evaluate(timer);
        intensity2 = pierceStartHighRumbleIntensityByTime.Evaluate(timer);

        while (timer < pierceStartLowRumbleIntensityByTime.keys[pierceStartLowRumbleIntensityByTime.length - 1].time)
        {
            intensity = pierceStartLowRumbleIntensityByTime.Evaluate(timer);
            intensity2 = pierceStartHighRumbleIntensityByTime.Evaluate(timer);
            gamepad.SetMotorSpeeds(intensity, intensity2);
            timer += motorsUpdateFrequence;
            yield return new WaitForSecondsRealtime(motorsUpdateFrequence);
        }

        intensity = pierceStartLowRumbleIntensityByTime.Evaluate(pierceStartLowRumbleIntensityByTime.keys[pierceStartLowRumbleIntensityByTime.length - 1].time);
        intensity2 = pierceStartHighRumbleIntensityByTime.Evaluate(pierceStartLowRumbleIntensityByTime.keys[pierceStartLowRumbleIntensityByTime.length - 1].time);
        gamepad.SetMotorSpeeds(intensity, intensity2);
        yield return new WaitForSecondsRealtime(4f);
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    private IEnumerator PierceEndRumble()
    {
        timer = 0;
        intensity = pierceEndLowRumbleIntensityByTime.Evaluate(timer);
        intensity2 = pierceEndHighRumbleIntensityByTime.Evaluate(timer);

        while (timer < pierceEndLowRumbleIntensityByTime.keys[pierceEndLowRumbleIntensityByTime.length - 1].time)
        {
            intensity = pierceEndLowRumbleIntensityByTime.Evaluate(timer);
            intensity2 = pierceEndHighRumbleIntensityByTime.Evaluate(timer);
            gamepad.SetMotorSpeeds(intensity, intensity2);
            timer += motorsUpdateFrequence;
            yield return new WaitForSecondsRealtime(motorsUpdateFrequence);
        }
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

    private IEnumerator TakeDamageRumble()
    {
        timer = 0;

        intensity = takeDamageRumbleIntensityByTime.Evaluate(timer);
        intensity2 = takeDamageHighRumbleIntensityByTime.Evaluate(timer);
        while (timer < takeDamageRumbleIntensityByTime.keys[takeDamageRumbleIntensityByTime.length - 1].time)
        {
            intensity = takeDamageRumbleIntensityByTime.Evaluate(timer);
            intensity2 = takeDamageHighRumbleIntensityByTime.Evaluate(timer);
            gamepad.SetMotorSpeeds(intensity, intensity2);
            timer += motorsUpdateFrequence;
            yield return new WaitForSecondsRealtime(motorsUpdateFrequence);
        }
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

    private IEnumerator DeathRumble()
    {
        timer = 0;
        intensity = deathLowRumbleIntensityByTime.Evaluate(timer);
        intensity2 = deathHighRumbleIntensityByTime.Evaluate(timer);
        while (timer < deathLowRumbleIntensityByTime.keys[deathLowRumbleIntensityByTime.length - 1].time)
        {
            intensity = deathLowRumbleIntensityByTime.Evaluate(timer);
            intensity2 = deathHighRumbleIntensityByTime.Evaluate(timer);
            gamepad.SetMotorSpeeds(intensity, intensity2);
            timer += motorsUpdateFrequence;
            yield return new WaitForSecondsRealtime(motorsUpdateFrequence);
        }
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

    private IEnumerator GrappleRumble()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        timer = 0;
        intensity = grappleLowRumbleIntensityByTime.Evaluate(timer);
        intensity2 = grappleHighRumbleIntensityByTime.Evaluate(timer);
        while (timer < grappleLowRumbleIntensityByTime.keys[grappleLowRumbleIntensityByTime.length - 1].time)
        {
            intensity = grappleLowRumbleIntensityByTime.Evaluate(timer);
            intensity2 = grappleHighRumbleIntensityByTime.Evaluate(timer);
            gamepad.SetMotorSpeeds(intensity, intensity2);
            timer += motorsUpdateFrequence;
            yield return new WaitForSecondsRealtime(motorsUpdateFrequence);
        }
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

    public static void StopAllRumbles()
    {
        I.gamepad.SetMotorSpeeds(0f, 0f);
    }

    void OnApplicationQuit()
    {
        StopAllRumbles();
    }

    public static void EnableRumbles(bool enable)
    {
        rumblesAreEnabled = enable;
        PlayerPrefs.SetInt("rumbles", enable ? 1 : 0);
    }
}
