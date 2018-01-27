using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceGameManager : MonoBehaviour {

    public int numPlayers;
    public int playersDead = 0;
    public Text victoryText;

    // Use this for initialization
    void Start () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("IcePlayer");
        for (int i = 3; i >= numPlayers; i--)
        {
            Destroy(players[i]);
        }
        victoryText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void handlePlayerDeath(string playerName) {
        playersDead++;
        if (playersDead >= numPlayers - 1) {
            victoryText.text = "Game Over!";
        }
    }
}
