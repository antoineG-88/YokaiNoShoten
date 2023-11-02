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
    [Header("ControlTexts")]
    public Text leftTriggerText;
    public Text rightTriggerText;
    public Text leftStickText;
    public Text rightStickText;
    public Text aButtonText;

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
        UpdateControlTexts();
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

    public void SelectObjectWithController(GameObject objectToSelect)
    {
        GameManager.eventSystem.firstSelectedGameObject = objectToSelect;
        GameManager.eventSystem.SetSelectedGameObject(objectToSelect);
    }
}
