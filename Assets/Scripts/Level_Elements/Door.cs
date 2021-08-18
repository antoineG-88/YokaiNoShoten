using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Switch connectedSwitch;
    private bool isOpened;
    private Collider2D doorCollider;
    public List<Animator> animators;
    private Animator animator;

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

    }

    private void Update()
    {
        if(connectedSwitch != null)
        {
            isOpened = connectedSwitch.IsON();
        }
        animator.SetBool("Opened", isOpened);
        doorCollider.enabled = !isOpened;
    }
}
