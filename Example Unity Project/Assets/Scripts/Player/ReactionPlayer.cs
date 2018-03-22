using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionPlayer : Player {

    [SerializeField]
    private ReactionGameManager reactionGameManager;

    private int roundWins;

    public int numPlayers;

    void Start () {
        
	}

    void Update () {
        HandleInput();
    }

    private void HandleInput()
    {
        if (_controls.GetKeyDownForUpKey()) {
            float pressTime = Time.timeSinceLevelLoad;
            reactionGameManager.Grab(_playerNumber, pressTime);
        }
    }
}
