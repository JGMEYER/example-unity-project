using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerControls
{

    float GetMovementHorizontal();

    float GetMovementVertical();

    bool GetSubmitDown();

    bool GetJump();

    bool GetUpKey();

    bool GetUpKeyDown();

    bool GetDownKey();

    bool GetDownKeyDown();

}