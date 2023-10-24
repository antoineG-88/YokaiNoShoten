using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Toggle rumbleToggle;

    private void Start()
    {
        rumbleToggle.isOn = RumblesManager.rumblesAreEnabled;
        fullscreenToggle.isOn = GameManager.isInFullScreen;
    }

    private IEnumerator UpdateToggles()
    {
        yield return new WaitForSeconds(0.2f);
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
}
