using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectEvent : EventPart
{
    public Sound soundEffectToPlay;
    public float delayTime;
    public bool waitSoundToContinueEvent;
    public bool scaleWithTimeScale;
    public AudioSource source;

    public override void StartEventPart()
    {
        base.StartEventPart();
        if(delayTime > 0)
        {
            StartCoroutine(Wait());
        }
        else
        {
            source.PlayOneShot(soundEffectToPlay.clip, soundEffectToPlay.volumeScale);
            StartCoroutine(SoundEnd());
        }

        if(!waitSoundToContinueEvent)
        {
            EndEventPart();
        }
    }

    private void Update()
    {
        if(scaleWithTimeScale)
        {
            source.pitch = Time.timeScale;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(delayTime);
        source.PlayOneShot(soundEffectToPlay.clip, soundEffectToPlay.volumeScale);
        StartCoroutine(SoundEnd());
    }
    private IEnumerator SoundEnd()
    {
        yield return new WaitForSeconds(soundEffectToPlay.clip.length);
        if (waitSoundToContinueEvent)
        {
            EndEventPart();
        }
    }
}
