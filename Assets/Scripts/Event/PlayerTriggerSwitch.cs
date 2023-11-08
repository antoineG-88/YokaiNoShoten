using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerSwitch : Switch
{
    public GameObject prompt;
    public GameObject gamepadPrompt;
    public GameObject keyboardPrompt;
    public EventTrigger eventToReset;

    private bool playerCanInteract;


    public override void Start()
    {
        base.Start();
        playerCanInteract = false;
        prompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerCanInteract = true;
            prompt.SetActive(true);
            gamepadPrompt.SetActive(GameManager.isUsingController);
            keyboardPrompt.SetActive(!GameManager.isUsingController);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerCanInteract = false;
            prompt.SetActive(false);
        }
    }

    public void Update()
    {
        if(playerCanInteract && (Input.GetButton("AButton") || Input.GetKeyDown(KeyCode.E)) && !eventToReset.isInEvent)
        {
            StartCoroutine(TempSwithOn());
        }
    }

    private IEnumerator TempSwithOn()
    {
        isOn = true;
        prompt.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        isOn = false;
        eventToReset.ResetEventWhenEnded();
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }
}
