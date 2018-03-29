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

    private bool upDownLast = false;
    private bool leftDownLast = false;
    private bool downDownLast = false;
    private bool rightDownLast = false;

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
        // Assumes value is polled to reset tracker
        bool upKey = GetAxis("DPadVertical") > 0;
        bool upKeyDown = upKey && !upDownLast;

        upDownLast = upKey;

        return upKeyDown;
    }

    bool IPlayerControls.GetLeftKey()
    {
        return GetAxis("DPadHorizontal") < 0;
    }

    bool IPlayerControls.GetLeftKeyDown()
    {
        // Assumes value is polled to reset tracker
        bool leftKey = GetAxis("DPadHorizontal") < 0;
        bool leftKeyDown = leftKey && !leftDownLast;

        leftDownLast = leftKey;

        return leftKeyDown;
    }

    bool IPlayerControls.GetDownKey()
    {
        return GetAxis("DPadVertical") < 0;
    }

    bool IPlayerControls.GetDownKeyDown()
    {
        // Assumes value is polled to reset tracker
        bool downKey = GetAxis("DPadVertical") < 0;
        bool downKeyDown = downKey && !downDownLast;

        downDownLast = downKey;

        return downKeyDown;
    }

    bool IPlayerControls.GetRightKey()
    {
        return GetAxis("DPadHorizontal") > 0;
    }

    bool IPlayerControls.GetRightKeyDown()
    {
        // Assumes value is polled to reset tracker
        bool rightKey = GetAxis("DPadHorizontal") > 0;
        bool rightKeyDown = rightKey && !rightDownLast;

        rightDownLast = rightKey;

        return rightKeyDown;
    }

    string IPlayerControls.GetJoinGameKeyName()
    {
        return "(A)";
    }
}
