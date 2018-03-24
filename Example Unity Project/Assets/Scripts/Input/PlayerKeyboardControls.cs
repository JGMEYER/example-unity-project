using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardControls : IPlayerControls
{

    // Only UI should pull these values. Players should call included methods
    // in order to keep all player input logic controller agnostic.
    public KeyCode UpKey { get; private set; }
    public KeyCode LeftKey { get; private set; }
    public KeyCode DownKey { get; private set; }
    public KeyCode RightKey { get; private set; }

    public PlayerKeyboardControls(KeyCode upKey, KeyCode leftKey, KeyCode downKey, KeyCode rightKey)
    {
        UpKey = upKey;
        LeftKey = leftKey;
        DownKey = downKey;
        RightKey = rightKey;
    }

    float IPlayerControls.GetMovementHorizontal()
    {
        float horizontal = 0f;

        if (Input.GetKey(LeftKey))
        {
            horizontal += -1;
        }
        if (Input.GetKey(RightKey))
        {
            horizontal += 1;
        }

        return horizontal;
    }

    float IPlayerControls.GetMovementVertical()
    {
        float vertical = 0f;

        if (Input.GetKey(UpKey))
        {
            vertical += 1;
        }
        if (Input.GetKey(DownKey))
        {
            vertical += -1;
        }

        return vertical;
    }

    bool IPlayerControls.GetSubmitDown()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }

    bool IPlayerControls.GetJump()
    {
        return Input.GetKey(UpKey);
    }

    bool IPlayerControls.GetUpKey()
    {
        return Input.GetKey(UpKey);
    }

    bool IPlayerControls.GetUpKeyDown()
    {
        return Input.GetKeyDown(UpKey);
    }

    bool IPlayerControls.GetDownKey()
    {
        return Input.GetKey(DownKey);
    }

    bool IPlayerControls.GetDownKeyDown()
    {
        return Input.GetKeyDown(DownKey);
    }

}
