using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot noGravityEffectSnapshot;
    public AudioMixer mainMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;

    private void Start()
    {
        if(!PlayerPrefs.HasKey("masterVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", 0.8f);
            PlayerPrefs.SetFloat("musicVolume", 0.8f);
            PlayerPrefs.SetFloat("effectVolume", 0.8f);
        }

        LoadVolumes();
    }

    public void EnableNoGravityMixerEffects()
    {
        noGravityEffectSnapshot.TransitionTo(0.1f);
    }

    public void DisableNoGravityMixerEffects()
    {
        defaultSnapshot.TransitionTo(0.1f);
    }

    public void SetMainVolume()
    {
        mainMixer.SetFloat("Volume", Mathf.Log10(masterVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
    }
    public void SetMusicVolume()
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
    }
    public void SetEffectVolume()
    {
        mainMixer.SetFloat("EffectVolume", Mathf.Log10(effectVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("effectVolume", effectVolumeSlider.value);
    }

    private void LoadVolumes()
    {
        mainMixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume")) * 20);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");

        mainMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume")) * 20);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");

        mainMixer.SetFloat("EffectVolume", Mathf.Log10(PlayerPrefs.GetFloat("effectVolume")) * 20);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("effectVolume");
    }
}
