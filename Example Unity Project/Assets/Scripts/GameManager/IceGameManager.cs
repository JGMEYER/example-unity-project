using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IceGameManager : MonoBehaviour {

	[SerializeField]
	private DEPRECATEDPlayerControlsController _playerControlsController;

	public int numPlayers;
	public int playersDead = 0;
	public Text victoryText;
    private bool gameOver;

	void Start() {
		InitializePlayers();
        gameOver = false;
    }

	public void HandlePlayerDeath(string playerName) {
		playersDead++;
		if (playersDead >= numPlayers - 1) {
			victoryText.text = "Game Over! Press ENTER to continue";
            gameOver = true;
		}
	}

	private void InitializePlayers() {
		IcePlayer[] players = Object.FindObjectsOfType(typeof(IcePlayer)) as IcePlayer[];
		for (int i = 3; i >= numPlayers; i--) {
			Destroy(players[i]);
		}
		victoryText.text = "";

		players = Object.FindObjectsOfType(typeof(IcePlayer)) as IcePlayer[];
		foreach (IcePlayer player in players) {
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

    void Update() {

		if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("GameSelect");
		}	

		if (gameOver && Input.GetKeyDown(KeyCode.KeypadEnter)) {
            SceneManager.LoadScene("GameSelect");
        }
    }

}
