using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Switch connectedSwitch;
    public List<Animator> animators;
    public Sound openingSound;
    public Sound closingSound;

    [HideInInspector] public bool isOpened;
    private Collider2D doorCollider;
    private Animator animator;
    private AudioSource source;
    private bool openFlag;

    private void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        for (int i = 0; i < animators.Count; i++)
        {
            animators[i].gameObject.SetActive(false);
        }

        switch(GameData.levelManager.zoneName)
        {
            case "Tuto":
                animator = animators[0];
                animators[0].gameObject.SetActive(true);
                break;

            case "SpacioDeck":
                animator = animators[1];
                animators[1].gameObject.SetActive(true);
                break;

            case "Labo":
                animator = animators[2];
                animators[2].gameObject.SetActive(true);
                break;

            case "Village":
                animator = animators[3];
                animators[3].gameObject.SetActive(true);
                break;

            default:
                animator = animators[0];
                animators[0].gameObject.SetActive(true);
                break;
        }

        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(connectedSwitch != null)
        {
            isOpened = connectedSwitch.IsON();
            if (isOpened != openFlag)
            {
                openFlag = isOpened;
                if(isOpened)
                {
                    if(openingSound.clip != null)
                        source.PlayOneShot(openingSound.clip, openingSound.volumeScale);
                }
                else
                {
                    if (closingSound.clip != null)
                        source.PlayOneShot(closingSound.clip, closingSound.volumeScale);
                }
            }
        }
        animator.SetBool("Opened", isOpened);
        doorCollider.enabled = !isOpened;
    }

    private void FixedUpdate()
    {
        source.pitch = Time.timeScale;
    }
}
