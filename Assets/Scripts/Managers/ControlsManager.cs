using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    [Header("Gamepad controls")]
    public bool aimMove;

    public static ControlsManager I;
    public static bool aimAndMovementSwitched;
    public static bool pierceAutoAimEnabled;
    public static bool grappleAndDashSwitched;
    public static bool pierceUseDashInput;
    public static bool altDashAndPierceAimEnabled;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("switchAimAndMovement"))
        {
            aimAndMovementSwitched = PlayerPrefs.GetInt("switchAimAndMovement") == 1 ? true : false;
        }
        else
        {
            SwitchGamepadAimAndMovement(false);
        }

        if (PlayerPrefs.HasKey("pierceAutoAim"))
        {
            pierceAutoAimEnabled = PlayerPrefs.GetInt("pierceAutoAim") == 1 ? true : false;
        }
        else
        {
            EnabledPierceAutoAim(false);
        }

        if (PlayerPrefs.HasKey("switchGrappleAndDash"))
        {
            grappleAndDashSwitched = PlayerPrefs.GetInt("switchGrappleAndDash") == 1 ? true : false;
        }
        else
        {
            SwitchGamepadGrappleAndDash(false);
        }

        if (PlayerPrefs.HasKey("pierceUseDashInput"))
        {
            pierceUseDashInput = PlayerPrefs.GetInt("pierceUseDashInput") == 1 ? true : false;
        }
        else
        {
            EnablePierceWithDashInput(true);
        }

        if (PlayerPrefs.HasKey("altDashPierceAim"))
        {
            altDashAndPierceAimEnabled = PlayerPrefs.GetInt("altDashPierceAim") == 1 ? true : false;
        }
        else
        {
            EnableDashAndPierceAltAim(false);
        }
    }

    public static void SwitchGamepadAimAndMovement(bool enable)
    {
        aimAndMovementSwitched = enable;
        PlayerPrefs.SetInt("switchAimAndMovement", enable ? 1 : 0);
        if(GameData.movementHandler != null)
        {
            GameData.movementHandler.moveWithRightJoystick = enable;
            GameData.grappleHandler.aimWithLeftJoystick = enable;
        }
    }

    public static void EnabledPierceAutoAim(bool enable)
    {
        pierceAutoAimEnabled = enable;
        PlayerPrefs.SetInt("pierceAutoAim", enable ? 1 : 0);
        if (GameData.pierceHandler != null)
        {
            GameData.pierceHandler.useAutoAim = enable;
        }
    }

    public static void SwitchGamepadGrappleAndDash(bool enable)
    {
        grappleAndDashSwitched = enable;
        PlayerPrefs.SetInt("switchGrappleAndDash", enable ? 1 : 0);
        if (GameData.grappleHandler != null)
        {
            GameData.grappleHandler.tractWithLeftTrigger = enable;
            GameData.dashHandler.dashWithRightTrigger = enable;
        }
    }

    public static void EnablePierceWithDashInput(bool enable)
    {
        pierceUseDashInput = enable;
        PlayerPrefs.SetInt("pierceUseDashInput", enable ? 1 : 0);
        if (GameData.pierceHandler != null)
        {
            GameData.pierceHandler.useDashInput = enable;
        }
    }

    public static void EnableDashAndPierceAltAim(bool enable)
    {
        altDashAndPierceAimEnabled = enable;
        PlayerPrefs.SetInt("altDashPierceAim", enable ? 1 : 0);
        if (GameData.dashHandler != null)
        {
            GameData.dashHandler.aimWithRightJoystick = enable;
            GameData.pierceHandler.aimWithRightJoystick = enable;
        }
    }
}
