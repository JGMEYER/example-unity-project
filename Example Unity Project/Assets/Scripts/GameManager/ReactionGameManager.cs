using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactionGameManager : GameManager<ReactionPlayer>
{

    [SerializeField]
    private ReactionSphere ReactionSphere;
    [SerializeField]
    private Text WinningPlayerText;
    [SerializeField]
    private ReactionToast ReactionToast;

    public int NumPlayers;
    public float MinWait;
    public float MaxWait;
    public float BetweenRoundTime;

    private bool gameOver;
    private bool roundActive;
    private bool allowGrab;
    private Dictionary<PlayerNumber, float> playerTimes;
    private float currentTime;

    private new void Awake()
    {
        base.Awake();

        gameOver = false;
        allowGrab = false;
        roundActive = false;
        playerTimes = new Dictionary<PlayerNumber, float>();
    }

    private new void Start()
    {
        base.Start();
        WinningPlayerText.text = "";
        StartRound();
    }

    private new void Update()
    {
        base.Update();

        if (!roundActive)
        {
            StartRound();
        }

        if (playerTimes.Count == NumPlayers)
        {
            allowGrab = false;
            EndRound();
        }
    }

    public void Grab(PlayerNumber playerNumber, float timePressed)
    {
        if (allowGrab && (!playerTimes.ContainsKey(playerNumber)))
        {
            Debug.Log("Grab! " + playerNumber);
            playerTimes.Add(playerNumber, timePressed);
        }
    }

    private void StartRound()
    {
        Debug.Log("Round started");
        roundActive = true;
        ReactionSphere.SetAsWaitColor();
        StartCoroutine(WaitForGrab());
    }

    private void EndRound()
    {
        float minTime = float.MaxValue;
        PlayerNumber player = PlayerNumber.One;
        foreach (KeyValuePair<PlayerNumber, float> pair in playerTimes)
        {
            if (pair.Value < minTime)
            {
                minTime = pair.Value;
                player = pair.Key;
            }
        }
        float timeTaken = minTime - currentTime;
        Debug.Log("Winner is player: " + player + " with time: " + timeTaken);
        WinningPlayerText.text = "Winner: " + player + " time: " + timeTaken;
        playerTimes.Clear();
        ReactionSphere.SetAsEndColor();
        StartCoroutine(WaitBetweenRounds());
    }


    private IEnumerator WaitForGrab()
    {
        float waitTime = Random.Range(MinWait, MaxWait);
        Debug.Log("Waiting for " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        allowGrab = true;
        currentTime = Time.timeSinceLevelLoad;
        ReactionSphere.SetAsStartColor();
        ReactionToast.flingToast();
        Debug.Log("Grab allowed");
    }

    private void EndGame(PlayerNumber playerNumber)
    {
        Debug.Log("Player " + playerNumber + "has won the game!");
        gameOver = true;
        StartCoroutine(EndGameAfterDelay());
    }

    private IEnumerator WaitBetweenRounds()
    {
        yield return new WaitForSeconds(BetweenRoundTime);
        roundActive = false;
        ReactionToast.resetToast();
    }
}
