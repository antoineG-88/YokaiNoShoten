using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlInstruction : MonoBehaviour
{
    public enum Instruction {Grapple, Dash, FallThrough, Pierce };

    public Instruction instruction;

    [Header("Refs")]
    public GameObject gamepad;
    public GameObject keyboard;
    public Animator rightStickAnimator;
    public Animator leftStickAnimator;
    public Animator rightTriggerAnimator;
    public Animator leftTriggerAnimator;
    public GameObject aButton;
    public GameObject mouseAim;
    public GameObject mouseClick;
    public Text mouseClickText;
    public GameObject rightClick;
    public GameObject leftClick;
    public SpriteRenderer singleKey;
    public Text singleKeyText;
    public Sprite regularKey;
    public Sprite spaceKey;


    private bool isGamepad;

    private void Start()
    {
        UpdateInstruction();
    }

    private void Update()
    {
        UpdateInstruction();
    }

    private void UpdateInstruction()
    {
        if(GameManager.isUsingController)
        {
            gamepad.SetActive(true);
            keyboard.SetActive(false);

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
                    if (ControlsManager.pierceUseDashInput)
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
        else
        {
            gamepad.SetActive(false);
            keyboard.SetActive(true);
            singleKey.gameObject.SetActive(false);
            mouseClickText.text = "";
            singleKey.sprite = regularKey;

            switch (instruction)
            {
                case Instruction.Grapple:
                    mouseAim.SetActive(true);

                    if(ControlsManager.grappleTractKey.ToString().Contains("Mouse"))
                    {
                        mouseClick.SetActive(true);
                        string clickName = ControlsManager.grappleTractKey.ToString();
                        if(clickName == "Mouse0")
                        {
                            clickName = "left click";
                            leftClick.SetActive(true);
                            rightClick.SetActive(false);
                        }
                        else if(clickName == "Mouse1")
                        {
                            clickName = "right click";
                            rightClick.SetActive(true);
                            leftClick.SetActive(false);
                        }
                        else
                        {
                            mouseClickText.text = clickName;
                        }
                    }
                    else
                    {
                        singleKey.gameObject.SetActive(true);
                        mouseClick.SetActive(false);
                        if (ControlsManager.grappleTractKey == KeyCode.Space)
                        {
                            singleKey.sprite = spaceKey;
                        }
                        singleKeyText.text = ControlsManager.grappleTractKey.ToString();
                    }
                    break;

                case Instruction.Dash:

                    if(ControlsManager.keyboardAimDashWithMouse)
                    {
                        mouseAim.SetActive(true);
                    }
                    else
                    {
                        mouseAim.SetActive(false);
                    }

                    if (ControlsManager.dashKey.ToString().Contains("Mouse"))
                    {
                        mouseClick.SetActive(true);
                        string clickName = ControlsManager.dashKey.ToString();
                        if (clickName == "Mouse0")
                        {
                            clickName = "left click";
                            leftClick.SetActive(true);
                            rightClick.SetActive(false);
                        }
                        else if (clickName == "Mouse1")
                        {
                            clickName = "right click";
                            rightClick.SetActive(true);
                            leftClick.SetActive(false);
                        }
                        else
                        {
                            mouseClickText.text = clickName;
                        }
                    }
                    else
                    {
                        singleKey.gameObject.SetActive(true);
                        mouseClick.SetActive(false);
                        if (ControlsManager.dashKey == KeyCode.Space)
                        {
                            singleKey.sprite = spaceKey;
                        }
                        singleKeyText.text = ControlsManager.dashKey.ToString();
                    }

                    break;

                case Instruction.FallThrough:
                    mouseAim.SetActive(false);

                    singleKey.gameObject.SetActive(true);
                    if (ControlsManager.downKey == KeyCode.Space)
                    {
                        singleKey.sprite = spaceKey;
                    }
                    singleKeyText.text = ControlsManager.downKey.ToString();

                    break;

                case Instruction.Pierce:
                    mouseAim.SetActive(false);
                    if (ControlsManager.pierceKey.ToString().Contains("Mouse"))
                    {
                        mouseClick.SetActive(true);
                        string clickName = ControlsManager.pierceKey.ToString();
                        if (clickName == "Mouse0")
                        {
                            clickName = "left click";
                            leftClick.SetActive(true);
                            rightClick.SetActive(false);
                        }
                        else if (clickName == "Mouse1")
                        {
                            clickName = "right click";
                            rightClick.SetActive(true);
                            leftClick.SetActive(false);
                        }
                        else
                        {
                            mouseClickText.text = clickName;
                        }
                    }
                    else
                    {
                        mouseClick.SetActive(false);
                        singleKey.gameObject.SetActive(true);
                        if (ControlsManager.pierceKey == KeyCode.Space)
                        {
                            singleKey.sprite = spaceKey;
                        }
                        singleKeyText.text = ControlsManager.pierceKey.ToString();
                    }
                    break;
            }
        }
    }
}
