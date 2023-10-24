using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageEffectManager : MonoBehaviour
{
    public VolumeProfile baseProfile;
    public VolumeProfile damageEffectStartProfile;
    public Volume volume;
    public AnimationCurve effectIntensity;
    public float effectTime;

    private Coroutine currentCoroutine;
    private float effectCurrentState;
    private ColorAdjustments colorAdjustments;
    private float colorAdjBaseSaturation;
    private Color colorAdjBaseColorFilter;
    private VolumeProfile effectProfile;


    void Start()
    {
        volume = GameObject.Find("PostProcessing").GetComponent<Volume>();
        baseProfile = volume.sharedProfile;

        effectCurrentState = 0;
        effectProfile = Instantiate(damageEffectStartProfile);

        effectProfile.TryGet<ColorAdjustments>(out colorAdjustments);

        colorAdjustments = Instantiate(colorAdjustments);

        for (int i = 0; i < effectProfile.components.Count; i++)
        {
            switch (effectProfile.components[i].name)
            {
                case "ColorAdjustments":
                    effectProfile.components[i] = colorAdjustments;
                    break;
            }
        }

        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.colorFilter.overrideState = true;

        colorAdjBaseSaturation = colorAdjustments.saturation.value;
        colorAdjBaseColorFilter = colorAdjustments.colorFilter.value;
    }

    public void StartDamageEffect()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(PlayDamageEffect());
    }

    private IEnumerator PlayDamageEffect()
    {
        volume.sharedProfile = effectProfile;

        float timer = 0;
        while(timer < effectTime)
        {
            effectCurrentState = effectIntensity.Evaluate(timer / effectTime);

            colorAdjustments.saturation.value = Mathf.Lerp(0, colorAdjBaseSaturation, effectCurrentState);
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, colorAdjBaseColorFilter, effectCurrentState);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        volume.sharedProfile = baseProfile;
    }

}
