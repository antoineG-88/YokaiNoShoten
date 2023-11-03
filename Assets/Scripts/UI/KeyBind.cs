using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class KeyBind : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Text keyText;
    public OptionManager.KeyboardBind actionBind;
    public OptionManager optionManager;

    private bool isSelected;
    [HideInInspector] public KeyCode keycodePressed;

    private void Start()
    {
        switch(actionBind)
        {
            case OptionManager.KeyboardBind.Up:
                keyText.text = ControlsManager.upKey.ToString();
                break;
            case OptionManager.KeyboardBind.Down:
                keyText.text = ControlsManager.downKey.ToString();
                break;
            case OptionManager.KeyboardBind.Right:
                keyText.text = ControlsManager.rightKey.ToString();
                break;
            case OptionManager.KeyboardBind.Left:
                keyText.text = ControlsManager.leftKey.ToString();
                break;
            case OptionManager.KeyboardBind.Grapple:
                keyText.text = ControlsManager.grappleTractKey.ToString();
                break;
            case OptionManager.KeyboardBind.Dash:
                keyText.text = ControlsManager.dashKey.ToString();
                break;
            case OptionManager.KeyboardBind.Pierce:
                keyText.text = ControlsManager.pierceKey.ToString();
                break;
        }
    }

    private void Update()
    {
        if(isSelected)
        {
            if(Input.anyKeyDown)
            {
                keycodePressed = KeyCode.None;
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                        keycodePressed = kcode;
                }

                if(keycodePressed != KeyCode.None)
                {
                    keyText.text = keycodePressed.ToString();
                    optionManager.ChangeKeyBind(actionBind, keycodePressed);
                }
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }
}
