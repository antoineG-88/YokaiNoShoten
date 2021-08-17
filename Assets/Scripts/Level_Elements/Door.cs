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

        switch(GameData.levelManager.zoneName)
        {
            case "Tuto":
                animator = animators[0];
                break;

            case "SpacioDeck":
                animator = animators[1];
                break;

            case "Labo":
                animator = animators[2];
                break;

            case "Village":
                animator = animators[3];
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
