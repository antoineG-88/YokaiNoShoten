using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicEvent : EventPart
{
    public AudioSource audioSource;
    [Tooltip("Choose null to stop the audio source")]
    public AudioClip newMusicToPlay;

    public override void StartEventPart()
    {
        base.StartEventPart();
        if(newMusicToPlay != null)
        {
            audioSource.clip = newMusicToPlay;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }

        EndEventPart();
    }
}
