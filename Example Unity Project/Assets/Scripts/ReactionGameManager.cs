using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionGameManager : MonoBehaviour {

    [SerializeField]
    private DEPRECATEDPlayerControlsController _playerControlsController;

    void Start() {
        InitializePlayers();
    }

    private void InitializePlayers() { 
        ReactionPlayer[] players = Object.FindObjectsOfType(typeof(ReactionPlayer)) as ReactionPlayer[];
        foreach (ReactionPlayer player in players) {
            // Using player name here is a hack because I don't know how to get a proper
            // player object from a tag. Make sure the object name matches the config.
            string playerUp = _playerControlsController.cfg[player.name]["Up"].StringValue;
            string playerLeft = _playerControlsController.cfg[player.name]["Left"].StringValue;
            string playerDown = _playerControlsController.cfg[player.name]["Down"].StringValue;
            string playerRight = _playerControlsController.cfg[player.name]["Right"].StringValue;
            player.upKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerUp);
            player.leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerLeft);
            player.downKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerDown);
            player.rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerRight);
        }
    }

    public void Grab() {
        Debug.Log("Grab!");
    }

    void Update() {

    }
}
