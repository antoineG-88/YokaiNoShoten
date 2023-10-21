using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public AudioSource source;
    public AudioSource musicSource;

    private void Start()
    {
        if(musicSource != null)
        {
            musicSource.PlayDelayed(1f);
        }
    }

    public void PlayUISound(AudioClip clip)
    {
        source.PlayOneShot(clip, 0.1f);
    }
}
