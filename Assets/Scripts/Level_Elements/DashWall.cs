using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashWall : MonoBehaviour
{
    public Sound passThroughSound;
    [SerializeField] private float disableTime = 0;

    public Collider2D wallCollider;
    private Animator animator;
    private bool isBeingDashed;
    private AudioSource source;

    void Start()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider == GameData.playerCollider)
        {

            if (GameData.dashHandler.isDashing)
            {
                if (!isBeingDashed)
                {
                    isBeingDashed = true;
                    animator.SetTrigger("Dashed");
                    source.PlayOneShot(passThroughSound.clip, passThroughSound.volumeScale);
                }
                StopAllCoroutines();
                StartCoroutine(DisableCollisionTemporary(collider));
            }


            /*if (GameData.grappleHandler.isTracting)
            {
                GameData.grappleHandler.BreakRope("Touched a dashWall");
            }*/
        }
    }

    private IEnumerator DisableCollisionTemporary(Collider2D collider)
    {
        Physics2D.IgnoreCollision(wallCollider, collider, true);
        yield return new WaitForSeconds(disableTime);
        Physics2D.IgnoreCollision(wallCollider, collider, false);
        isBeingDashed = false;
    }
}
