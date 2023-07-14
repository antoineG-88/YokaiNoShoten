using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerSwitch : Switch
{
    public SpriteRenderer prompt;
    public EventTrigger eventToReset;

    private bool playerCanInteract;


    public override void Start()
    {
        base.Start();
        playerCanInteract = false;
        prompt.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerCanInteract = true;
            prompt.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerCanInteract = false;
            prompt.enabled = false;
        }
    }

    public void Update()
    {
        if(playerCanInteract && Input.GetAxisRaw("LeftTrigger") == 1 && !eventToReset.isInEvent)
        {
            StartCoroutine(TempSwithOn());
        }
    }

    private IEnumerator TempSwithOn()
    {
        isOn = true;
        prompt.enabled = false;
        yield return new WaitForSeconds(0.5f);
        isOn = false;
        eventToReset.ResetEventWhenEnded();
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }
}
