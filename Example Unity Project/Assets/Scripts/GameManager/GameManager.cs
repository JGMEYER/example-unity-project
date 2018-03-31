using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager<P> : MonoBehaviour where P : Player
{

    // Singleton values
    private static GameManager<P> instance;
    public static GameManager<P> Instance { get { return instance; } }

    private const string GameSelect = "GameSelect";

    protected GlobalControls globalControls;
    protected P[] players;
    protected int numPlayers { get; private set; }

    protected bool roundActive { get; private set; }
    protected bool gameOver { get; private set; }

    protected void Awake()
    {
        // GameManager is a singleton, load only one per scene
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        globalControls = InputManager.Instance.GlobalControls();
        numPlayers = InputManager.Instance.NumPlayersRegistered();

        roundActive = false;
        gameOver = false;

        InitializePlayersAndRemoveExtra();
    }

    protected void Start() { }

    protected void Update()
    {
        DoInput();
    }

    private void InitializePlayersAndRemoveExtra()
    {
        List<P> playersInScene = new List<P>(FindObjectsOfType(typeof(P)) as P[]);
        for (int i = playersInScene.Count - 1; i >= 0; i--)
        {
            P player = playersInScene[i];
            if ((int)player.GetPlayerNumber() > numPlayers)
            {
                playersInScene.RemoveAt(i);
                Destroy(player.gameObject);
            }
        }

        players = playersInScene.ToArray();
    }

    protected void DoInput()
    {
        if (globalControls.GetExitKeyDown())
        {
            SceneManager.LoadSceneAsync(GameSelect);
        }
    }

    // =================
    // Game Flow Logic
    // =================

    // Only ever extend these, do not fully override or call them directly

    protected virtual void StartRound()
    {
        Debug.Log("Round started");
        roundActive = true;

        foreach (Player player in players)
        {
            player.SetActive(true);
        }
    }

    protected virtual void EndRound()
    {
        Debug.Log("Round ended");
        roundActive = false;

        foreach (Player player in players)
        {
            player.SetActive(false);
        }
    }

    protected virtual void ResetRound() {
        Debug.Log("Round reset");
        // game defined
    }

    protected virtual void EndGame(PlayerNumber[] winners)
    {
        if (winners == null || winners.Length == 0)
        {
            Debug.LogError("Empty array of winners passed. Pass winners to " +
                "EndGame() to properly handle scoring for your game.");
        }
        else if (winners.Length == 1)
        {
            Debug.Log("Player " + winners[0] + " wins!");
        }
        else
        {
            string winnersString = string.Join(", ", winners.Select(winner => winner.ToString()).ToArray());
            Debug.Log("Players " + winnersString + " have won the game!");
        }

        gameOver = true;
    }

    // Call these to control your game flow

    protected IEnumerator StartRoundAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        StartRound();
    }

    protected IEnumerator ResetRoundAfterDelay()
    {
        EndRound();
        yield return new WaitForSeconds(2f);
        ResetRound();
        StartRound();
    }

    protected IEnumerator EndGameAfterDelay(PlayerNumber[] winners)
    {
        EndRound();
        EndGame(winners);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync(GameSelect);
    }

}
