using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControls : Object
{

    public KeyCode SubmitKey { get; private set; }
    public KeyCode ExitKey { get; private set; }

    public GlobalControls(KeyCode submitKey, KeyCode exitKey)
    {
        SubmitKey = submitKey;
        ExitKey = exitKey;
    }

    public bool GetSubmitKeyDown()
    {
        return Input.GetKeyDown(SubmitKey);
    }

    public bool GetExitKeyDown()
    {
        return Input.GetKeyDown(ExitKey);
    }

}
