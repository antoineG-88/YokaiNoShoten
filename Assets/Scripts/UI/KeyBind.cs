using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class KeyBind : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerExitHandler
{
    public Text keyText;
    public OptionManager.KeyboardBind actionBind;
    public OptionManager optionManager;
    public GameObject pressKeyInstruction;

    private bool isSelected;
    [HideInInspector] public KeyCode keycodePressed;
    private KeyCode currentKeycode;

    private void Start()
    {
        switch(actionBind)
        {
            case OptionManager.KeyboardBind.Up:
                keyText.text = GetBetterKeyName(ControlsManager.upKey);
                currentKeycode = ControlsManager.upKey;
                break;
            case OptionManager.KeyboardBind.Down:
                keyText.text = GetBetterKeyName(ControlsManager.downKey);
                currentKeycode = ControlsManager.downKey;
                break;
            case OptionManager.KeyboardBind.Right:
                keyText.text = GetBetterKeyName(ControlsManager.rightKey);
                currentKeycode = ControlsManager.rightKey;
                break;
            case OptionManager.KeyboardBind.Left:
                keyText.text = GetBetterKeyName(ControlsManager.leftKey);
                currentKeycode = ControlsManager.leftKey;
                break;
            case OptionManager.KeyboardBind.Grapple:
                keyText.text = GetBetterKeyName(ControlsManager.grappleTractKey);
                currentKeycode = ControlsManager.grappleTractKey;
                break;
            case OptionManager.KeyboardBind.Dash:
                keyText.text = GetBetterKeyName(ControlsManager.dashKey);
                currentKeycode = ControlsManager.dashKey;
                break;
            case OptionManager.KeyboardBind.Pierce:
                keyText.text = GetBetterKeyName(ControlsManager.pierceKey);
                currentKeycode = ControlsManager.pierceKey;
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
                    keyText.text = GetBetterKeyName(keycodePressed);
                    optionManager.ChangeKeyBind(actionBind, keycodePressed);
                    currentKeycode = keycodePressed;
                    GameManager.eventSystem.SetSelectedGameObject(null);
                }
            }
        }
    }

    private string GetBetterKeyName(KeyCode key)
    {
        string keyName = key.ToString();
        if (keyName == "Mouse0")
        {
            keyName = "Left click";
        }
        else if (keyName == "Mouse1")
        {
            keyName = "Right click";
        }
        return keyName;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        pressKeyInstruction.SetActive(true);
        keyText.text = "";
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        pressKeyInstruction.SetActive(false);
        keyText.text = GetBetterKeyName(currentKeycode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.eventSystem.SetSelectedGameObject(null);
    }
}
