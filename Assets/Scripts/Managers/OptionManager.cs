using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionManager : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle fullscreenToggle;
    public Toggle rumbleToggle;
    public Toggle aimMovementSwitchToggle;
    public Toggle grappleDashSwitchToggle;
    public Toggle pierceAutoAimToggle;
    public Toggle pierceUseDashInputToggle;
    public Toggle dashPierceAltAimToggle;
    public Toggle keyboardDashAimWithMouse;
    [Header("ControlTexts")]
    public Text leftTriggerText;
    public Text rightTriggerText;
    public Text leftStickText;
    public Text rightStickText;
    public Text aButtonText;

    public enum KeyboardBind {Up, Down, Right, Left, Grapple, Dash, Pierce };

    private Button currentBackButton;

    private void Start()
    {
        rumbleToggle.isOn = RumblesManager.rumblesAreEnabled;
        fullscreenToggle.isOn = GameManager.isInFullScreen;
        aimMovementSwitchToggle.isOn = ControlsManager.aimAndMovementSwitched;
        grappleDashSwitchToggle.isOn = ControlsManager.grappleAndDashSwitched;
        pierceAutoAimToggle.isOn = ControlsManager.pierceAutoAimEnabled;
        pierceUseDashInputToggle.isOn = ControlsManager.pierceUseDashInput;
        dashPierceAltAimToggle.isOn = ControlsManager.altDashAndPierceAimEnabled;
        keyboardDashAimWithMouse.isOn = ControlsManager.keyboardAimDashWithMouse;
        UpdateControlTexts();
    }

    private void Update()
    {
        if(Input.GetButtonDown("BButton"))
        {
            if(currentBackButton != null)
            {
                currentBackButton.onClick.Invoke();
            }
        }
    }

    private IEnumerator UpdateToggles()
    {
        yield return new WaitForSeconds(0.2f);
    }


    public void UpdateControlTexts()
    {
        leftTriggerText.text = ControlsManager.grappleAndDashSwitched ? "Grapple" : (ControlsManager.pierceUseDashInput ? "Dash / Pierce" : "Dash");
        rightTriggerText.text = ControlsManager.grappleAndDashSwitched ? (ControlsManager.pierceUseDashInput ? "Dash / Pierce" : "Dash") : "Grapple";
        leftStickText.text = (ControlsManager.aimAndMovementSwitched ? "Aim grapple" : "Move") + (ControlsManager.altDashAndPierceAimEnabled ? "" : " / Aim dash and pierce");
        rightStickText.text = (ControlsManager.aimAndMovementSwitched ? "Move" : "Aim grapple") + (ControlsManager.altDashAndPierceAimEnabled ? " / Aim dash and pierce" : "");
        aButtonText.text = "Validate" + (ControlsManager.pierceUseDashInput ? "" : " / Pierce");
    }

    public void ChangeKeyBind(KeyboardBind keyboardBind, KeyCode key)
    {
        switch(keyboardBind)
        {
            case KeyboardBind.Up:
                ControlsManager.SetUpKey(key);
                break;

            case KeyboardBind.Down:
                ControlsManager.SetDownKey(key);
                break;

            case KeyboardBind.Right:
                ControlsManager.SetRightKey(key);
                break;

            case KeyboardBind.Left:
                ControlsManager.SetLeftKey(key);
                break;

            case KeyboardBind.Grapple:
                ControlsManager.SetTractGrappleKey(key);
                break;

            case KeyboardBind.Dash:
                ControlsManager.SetDashKey(key);
                break;

            case KeyboardBind.Pierce:
                ControlsManager.SetPierceKey(key);
                break;
        }
    }

    public void ChangeScreenMode(bool value)
    {
        GameManager.ToggleFullScreen(value);
    }

    public void ToggleRumbles(bool value)
    {
        RumblesManager.EnableRumbles(value);
        RumblesManager.EndPhasingRumble();
    }

    public void ToggleSwitchAimAndMovement(bool value)
    {
        ControlsManager.SwitchGamepadAimAndMovement(value);
        UpdateControlTexts();
    }

    public void ToggleSwitchGrappleAndDash(bool value)
    {
        ControlsManager.SwitchGamepadGrappleAndDash(value);
        UpdateControlTexts();
    }

    public void TogglePierceAutoAim(bool value)
    {
        ControlsManager.EnabledPierceAutoAim(value);
    }

    public void TogglePierecUseDashInput(bool value)
    {
        ControlsManager.EnablePierceWithDashInput(value);
        UpdateControlTexts();
    }

    public void ToggleAltDashAndPierceAim(bool value)
    {
        ControlsManager.EnableDashAndPierceAltAim(value);
        UpdateControlTexts();
    }
    public void ToggleKeyboardDashAimWithMouse(bool value)
    {
        ControlsManager.SwitchKeyboardDashAim(value);
    }

    public void SelectObjectWithController(GameObject objectToSelect)
    {
        GameManager.eventSystem.firstSelectedGameObject = objectToSelect;
        GameManager.eventSystem.SetSelectedGameObject(objectToSelect);
    }
    public void SetCurrentBackButotn(Button backButton)
    {
        currentBackButton = backButton;
    }
}
