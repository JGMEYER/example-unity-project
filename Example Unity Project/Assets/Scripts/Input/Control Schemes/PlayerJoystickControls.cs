using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoystickControls : IPlayerControls
{

    public int JoystickNumber { get; private set;}

    public PlayerJoystickControls(int joystickNumber)
    {
        JoystickNumber = joystickNumber;
    }

    // ==============
    // Axes Helpers
    // ==============

    private String GetJoystickAxisName(String axisName)
    {
        return "Joystick" + JoystickNumber + axisName;
    }

    private float GetAxis(String axisName)
    {
        return Input.GetAxis(GetJoystickAxisName(axisName));
    }

    private float GetAxisRaw(String axisName)
    {
        return Input.GetAxisRaw(GetJoystickAxisName(axisName));
    }

    // ================
    // Button Helpers
    // ================

    private KeyCode GetJoystickButtonKeyCode(int buttonNum)
    {
        String keyName = "Joystick" + JoystickNumber + "Button" + buttonNum;
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);
    }

    private bool GetButton(int buttonNum)
    {
        KeyCode keyCode = GetJoystickButtonKeyCode(buttonNum);
        return Input.GetKey(keyCode);
    }

    private bool GetButtonDown(int buttonNum)
    {
        KeyCode keyCode = GetJoystickButtonKeyCode(buttonNum);
        return Input.GetKeyDown(keyCode);
    }

    // ===================
    // Interface Methods
    // ===================

    float IPlayerControls.GetMovementHorizontal()
    {
        float leftAxis = GetAxisRaw("LeftHorizontal");
        float dPadAxis = GetAxisRaw("DPadHorizontal");

        return Mathf.Clamp(leftAxis + dPadAxis, -1f, 1f);
    }

    float IPlayerControls.GetMovementVertical()
    {
        float leftAxis = GetAxisRaw("LeftVertical");
        float dPadAxis = GetAxisRaw("DPadVertical");

        return Mathf.Clamp(leftAxis + dPadAxis, -1f, 1f);
    }

    bool IPlayerControls.GetSubmitDown()
    {
        return GetButtonDown(0);
    }

    bool IPlayerControls.GetJoinGameDown()
    {
        return GetButtonDown(0);
    }

    bool IPlayerControls.GetJump()
    {
        return GetButton(0);
    }

    bool IPlayerControls.GetUpKey()
    {
        return GetAxis("DPadVertical") > 0;
    }

    bool IPlayerControls.GetUpKeyDown()
    {
        return GetAxis("DPadVertical") > 0;  // incorrect event!
    }

    bool IPlayerControls.GetDownKey()
    {
        return GetAxis("DPadVertical") < 0;
    }

    bool IPlayerControls.GetDownKeyDown()
    {
        return GetAxis("DPadVertical") < 0;  // incorrect event!
    }

    string IPlayerControls.GetJoinGameKeyName()
    {
        return "(A)";
    }

}
