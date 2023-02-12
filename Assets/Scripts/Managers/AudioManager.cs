using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot noGravityEffectSnapshot;

    public void EnableNoGravityMixerEffects()
    {
        noGravityEffectSnapshot.TransitionTo(0.1f);
    }

    public void DisableNoGravityMixerEffects()
    {
        defaultSnapshot.TransitionTo(0.1f);
    }
}
