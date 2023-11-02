using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlInstruction : MonoBehaviour
{
    public enum Instruction {Grapple, Dash, FallThrough, Pierce };

    public Instruction instruction;

    [Header("Refs")]
    public Animator rightStickAnimator;
    public Animator leftStickAnimator;
    public Animator rightTriggerAnimator;
    public Animator leftTriggerAnimator;
    public GameObject aButton;

    private void Update()
    {
        UpdateInstruction();
    }

    private void UpdateInstruction()
    {
        rightTriggerAnimator.SetBool("Used", false);
        leftTriggerAnimator.SetBool("Used", false);
        rightStickAnimator.SetInteger("UseState", 0);
        leftStickAnimator.SetInteger("UseState", 0);
        aButton.SetActive(false);

        switch (instruction)
        {
            case Instruction.Grapple:
                if (ControlsManager.grappleAndDashSwitched)
                {
                    leftTriggerAnimator.SetBool("Used", true);
                }
                else
                {
                    rightTriggerAnimator.SetBool("Used", true);
                }

                if (ControlsManager.aimAndMovementSwitched)
                {
                    leftStickAnimator.SetInteger("UseState", 1);
                }
                else
                {
                    rightStickAnimator.SetInteger("UseState", 1);
                }
                break;

            case Instruction.Dash:
                if (ControlsManager.grappleAndDashSwitched)
                {
                    rightTriggerAnimator.SetBool("Used", true);
                }
                else
                {
                    leftTriggerAnimator.SetBool("Used", true);
                }

                if (ControlsManager.altDashAndPierceAimEnabled)
                {
                    rightStickAnimator.SetInteger("UseState", 1);
                }
                else
                {
                    leftStickAnimator.SetInteger("UseState", 1);
                }
                break;

            case Instruction.FallThrough:
                if (ControlsManager.aimAndMovementSwitched)
                {
                    rightStickAnimator.SetInteger("UseState", 2);
                }
                else
                {
                    leftStickAnimator.SetInteger("UseState", 2);
                }
                break;

            case Instruction.Pierce:
                if(ControlsManager.pierceUseDashInput)
                {
                    if (ControlsManager.grappleAndDashSwitched)
                    {
                        rightTriggerAnimator.SetBool("Used", true);
                    }
                    else
                    {
                        leftTriggerAnimator.SetBool("Used", true);
                    }
                }
                else
                {
                    aButton.SetActive(true);
                }

                if (ControlsManager.altDashAndPierceAimEnabled)
                {
                    rightStickAnimator.SetInteger("UseState", 1);
                }
                else
                {
                    leftStickAnimator.SetInteger("UseState", 1);
                }
                break;
        }
    }
}
