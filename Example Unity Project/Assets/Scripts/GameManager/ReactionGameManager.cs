﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ReactionGameManager : MonoBehaviour {


    public int numPlayers;
    private bool gameOver;
    private bool roundActive;
    private bool allowGrab;
    private Dictionary<PlayerNumber, float> playerTimes;
    public float minWait;
    public float maxWait;

    void Start() {
        gameOver = false;
        allowGrab = false;
        roundActive = false;
        playerTimes = new Dictionary<PlayerNumber, float>();
        StartRound();
    }

    public void Grab(PlayerNumber playerNumber, float timePressed) {
        if (allowGrab && (!playerTimes.ContainsKey(playerNumber))) {
            Debug.Log("Grab! " + playerNumber);
            playerTimes.Add(playerNumber, timePressed);
        }
    }

    void Update() {
        if (!roundActive) {
            StartRound();
        }
        if (playerTimes.Count == numPlayers) {
            allowGrab = false;
            EndRound();
        }
    }

    private void StartRound() {
        Debug.Log("Round started");
        roundActive = true;
        //reset screen elements here
        StartCoroutine(Wait());
    }

    private void EndRound() {
        float minTime = float.MaxValue;
        PlayerNumber player = PlayerNumber.ONE;
        foreach (KeyValuePair<PlayerNumber, float> pair in playerTimes) {
            if (pair.Value < minTime) {
                minTime = pair.Value;
                player = pair.Key;
            }
        }
        //end round and display here, need to update player score
        Debug.Log("Winner is player: " + player + " with time: " + minTime);
        playerTimes.Clear();
        roundActive = false;
    }


    IEnumerator Wait()
    {
        float waitTime = Random.Range(minWait, maxWait);
        Debug.Log("Waiting for " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        //fire off screen elements here
        allowGrab = true;
        Debug.Log("Grab allowed");
    }

    private void EndGame(PlayerNumber playerNumber) {
        Debug.Log("Player " + playerNumber + "has won the game!");
        gameOver = true;
    }
}
