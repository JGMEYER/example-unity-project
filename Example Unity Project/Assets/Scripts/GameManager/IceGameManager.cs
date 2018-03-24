using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IceGameManager : GameManager<IcePlayer>
{

    [SerializeField]
    private Text victoryText;

    public int NumPlayers;

    private bool gameOver;
    private int playersDead = 0;

    private new void Start()
    {
        base.Start();

        RemoveExtraPlayersFromScene();
        victoryText.text = "";
    }

    public void HandlePlayerDeath(string playerName)
    {
        playersDead++;

        if (playersDead >= NumPlayers - 1)
        {
            victoryText.text = "Game Over!";
            gameOver = true;

            StartCoroutine(EndGameAfterDelay());
        }
    }

    // This logic (or similar) should move to GameManager once we have a proper
    // player join screen and a system to track number of players.
    private void RemoveExtraPlayersFromScene()
    {
        List<IcePlayer> activePlayers = new List<IcePlayer>(players);

        for (int i = 3; i >= NumPlayers; i--)
        {
            IcePlayer playerToRemove = players[i];
            activePlayers.RemoveAt(i);
            Destroy(players[i]);
        }

        players = activePlayers.ToArray();
    }

}
