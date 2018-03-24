using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IceGameManager : GameManager<IcePlayer>
{

    public int numPlayers;

    [SerializeField]
    private Text _victoryText;

    private bool _gameOver;
    private int _playersDead = 0;

    new void Start()
    {
        base.Start();

        RemoveExtraPlayersFromScene();
        _victoryText.text = "";
    }

    public void HandlePlayerDeath(string playerName)
    {
        _playersDead++;

        if (_playersDead >= numPlayers - 1)
        {
            _victoryText.text = "Game Over!";
            _gameOver = true;

            StartCoroutine(EndGameAfterDelay());
        }
    }

    // This logic (or similar) should move to GameManager once we have a proper
    // player join screen and a system to track number of players.
    void RemoveExtraPlayersFromScene()
    {
        List<IcePlayer> activePlayers = new List<IcePlayer>(players);

        for (int i = 3; i >= numPlayers; i--)
        {
            IcePlayer playerToRemove = players[i];
            activePlayers.RemoveAt(i);
            Destroy(players[i]);
        }

        players = activePlayers.ToArray();
    }

}
