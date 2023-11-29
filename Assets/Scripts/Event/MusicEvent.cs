using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicEvent : EventPart
{
    public AudioSource audioSource;
    public bool fadeOutPreviousMusic = true;
    public float fadeOutTime = 0.8f;
    [Tooltip("Choose null to stop the audio source")]
    public AudioClip newMusicToPlay;

    private float sourceOriginalVolume;

    public override void StartEventPart()
    {
        base.StartEventPart();
        sourceOriginalVolume = audioSource.volume;

        if (fadeOutPreviousMusic)
        {
            StartCoroutine(FadeOutMusic());
        }
        else
        {
            StartNewMusic();
        }

        EndEventPart();
    }

    private void StartNewMusic()
    {
        if (newMusicToPlay != null)
        {
            audioSource.clip = newMusicToPlay;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    public IEnumerator FadeOutMusic()
    {
        float timer = 0;
        while (timer < fadeOutTime)
        {
            audioSource.volume = (1 - (timer / fadeOutTime)) * sourceOriginalVolume;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        audioSource.Stop();
        audioSource.volume = sourceOriginalVolume;

        StartNewMusic();
    }
}
