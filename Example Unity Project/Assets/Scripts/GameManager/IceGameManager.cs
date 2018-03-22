using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IceGameManager : MonoBehaviour {

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
		IcePlayer[] players = FindObjectsOfType(typeof(IcePlayer)) as IcePlayer[];
		for (int i = 3; i >= numPlayers; i--) {
			Destroy(players[i]);
		}
		victoryText.text = "";

		players = FindObjectsOfType(typeof(IcePlayer)) as IcePlayer[];
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
