using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicEvent : EventPart
{
    public AudioSource audioSource;
    public AudioClip newMusicToPlay;

    public override void StartEventPart()
    {
        base.StartEventPart();
        audioSource.clip = newMusicToPlay;
        audioSource.Play();

        EndEventPart();
    }
}
