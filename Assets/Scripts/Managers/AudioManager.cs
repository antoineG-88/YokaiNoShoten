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
        mainMixer.SetFloat("Volume", Mathf.Log(masterVolumeSlider.value, 6) * 30);
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
    }
    public void SetMusicVolume()
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log(musicVolumeSlider.value, 6) * 30);
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
    }
    public void SetEffectVolume()
    {
        mainMixer.SetFloat("EffectVolume", Mathf.Log(effectVolumeSlider.value, 6) * 30);
        PlayerPrefs.SetFloat("effectVolume", effectVolumeSlider.value);
    }

    private void LoadVolumes()
    {
        mainMixer.SetFloat("Volume", Mathf.Log(PlayerPrefs.GetFloat("masterVolume"), 6) * 30);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");

        mainMixer.SetFloat("MusicVolume", Mathf.Log(PlayerPrefs.GetFloat("musicVolume"), 6) * 30);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");

        mainMixer.SetFloat("EffectVolume", Mathf.Log(PlayerPrefs.GetFloat("effectVolume"), 6) * 30);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("effectVolume");
    }
}
