using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControls : Object
{

    public KeyCode ExitKey { get; private set; }

    public GlobalControls(KeyCode exitKey)
    {
        ExitKey = exitKey;
    }

    public bool GetExitKeyDown()
    {
        return Input.GetKeyDown(ExitKey);
    }

}
