using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Switch connectedSwitch;
    private bool isOpened;
    private Collider2D doorCollider;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
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
