using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceGameManager : MonoBehaviour {

    public int numPlayers;
    public int playersDead = 0;
    public Text victoryText;

    void Start () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("IcePlayer");
        for (int i = 3; i >= numPlayers; i--) {
            Destroy(players[i]);
        }
        victoryText.text = "";
	}

    public void handlePlayerDeath(string playerName) {
        playersDead++;
        if (playersDead >= numPlayers - 1) {
            victoryText.text = "Game Over!";
        }
    }

}
