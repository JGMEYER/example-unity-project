using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionPlayer : Player
{

    [SerializeField]
    private ReactionGameManager reactionGameManager;

    private int roundWins;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (controls.GetUpKeyDown())
        {
            float pressTime = Time.timeSinceLevelLoad;
            reactionGameManager.Grab(playerNumber, pressTime);
        }
    }

}
