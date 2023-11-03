using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public bool deletePlayerPrefsOnStart;
    [Header("Keyboard default bindings")]
    public KeyCode tractGrappleDefaultKey;
    public KeyCode dashDefaultKey;
    public KeyCode pierceDefaultKey;
    public KeyCode upDefaultKey;
    public KeyCode downDefaultKey;
    public KeyCode rightDefaultKey;
    public KeyCode leftDefaultKey;

    public static ControlsManager I;
    public static bool aimAndMovementSwitched;
    public static bool pierceAutoAimEnabled;
    public static bool grappleAndDashSwitched;
    public static bool pierceUseDashInput;
    public static bool altDashAndPierceAimEnabled;

    public static KeyCode grappleTractKey;
    public static KeyCode dashKey;
    public static KeyCode pierceKey;
    public static KeyCode upKey;
    public static KeyCode downKey;
    public static KeyCode rightKey;
    public static KeyCode leftKey;
    public static bool keyboardAimDashWithMouse;

    private void Awake()
    {
        if(deletePlayerPrefsOnStart)
            PlayerPrefs.DeleteAll();

        SetupGamepadOptions();
        SetupKeyboardMapping();
    }

    private void SetupKeyboardMapping()
    {
        if (PlayerPrefs.HasKey("grappleTractKey"))
        {
            grappleTractKey = (KeyCode)PlayerPrefs.GetInt("grappleTractKey");
        }
        else
        {
            SetTractGrappleKey(tractGrappleDefaultKey);
        }

        if (PlayerPrefs.HasKey("dashKey"))
        {
            dashKey = (KeyCode)PlayerPrefs.GetInt("dashKey");
        }
        else
        {
            SetDashKey(dashDefaultKey);
        }

        if (PlayerPrefs.HasKey("pierceKey"))
        {
            pierceKey = (KeyCode)PlayerPrefs.GetInt("pierceKey");
        }
        else
        {
            SetPierceKey(pierceDefaultKey);
        }

        if (PlayerPrefs.HasKey("upKey"))
        {
            upKey = (KeyCode)PlayerPrefs.GetInt("upKey");
        }
        else
        {
            SetUpKey(upDefaultKey);
        }

        if (PlayerPrefs.HasKey("downKey"))
        {
            downKey = (KeyCode)PlayerPrefs.GetInt("downKey");
        }
        else
        {
            SetDownKey(downDefaultKey);
        }

        if (PlayerPrefs.HasKey("rightKey"))
        {
            rightKey = (KeyCode)PlayerPrefs.GetInt("rightKey");
        }
        else
        {
            SetRightKey(rightDefaultKey);
        }

        if (PlayerPrefs.HasKey("leftKey"))
        {
            leftKey = (KeyCode)PlayerPrefs.GetInt("leftKey");
        }
        else
        {
            SetLeftKey(leftDefaultKey);
        }
    }

    private void SetupGamepadOptions()
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

        if (PlayerPrefs.HasKey("keyboardDashAimWithMouse"))
        {
            keyboardAimDashWithMouse = PlayerPrefs.GetInt("keyboardDashAimWithMouse") == 1 ? true : false;
        }
        else
        {
            SwitchKeyboardDashAim(false);
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

    public static void SwitchKeyboardDashAim(bool enable)
    {
        keyboardAimDashWithMouse = enable;
        PlayerPrefs.SetInt("keyboardDashAimWithMouse", enable ? 1 : 0);
        if (GameData.dashHandler != null)
        {
            GameData.dashHandler.keyboardAimWithMouse = enable;
        }
    }

    public static void SetTractGrappleKey(KeyCode key)
    {
        grappleTractKey = key;
        PlayerPrefs.SetInt("grappleTractKey", (int)key);
    }

    public static void SetDashKey(KeyCode key)
    {
        dashKey = key;
        PlayerPrefs.SetInt("dashKey", (int)key);
    }

    public static void SetPierceKey(KeyCode key)
    {
        pierceKey = key;
        PlayerPrefs.SetInt("pierceKey", (int)key);
    }

    public static void SetUpKey(KeyCode key)
    {
        upKey = key;
        PlayerPrefs.SetInt("upKey", (int)key);
    }

    public static void SetDownKey(KeyCode key)
    {
        downKey = key;
        PlayerPrefs.SetInt("downKey", (int)key);
    }

    public static void SetRightKey(KeyCode key)
    {
        rightKey = key;
        PlayerPrefs.SetInt("rightKey", (int)key);
    }

    public static void SetLeftKey(KeyCode key)
    {
        leftKey = key;
        PlayerPrefs.SetInt("leftKey", (int)key);
    }
}
