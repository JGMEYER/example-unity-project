using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactionGameManager : GameManager<ReactionPlayer>
{

    public int numPlayers;
    private bool gameOver;
    private bool roundActive;
    private bool allowGrab;
    private Dictionary<PlayerNumber, float> playerTimes;
    public float minWait;
    public float maxWait;
    private float currentTime;
    public float betweenRoundTime;
    [SerializeField]
    private ReactionSphere reactionSphere;
    [SerializeField]
    private Text winningPlayerText;

    new void Awake()
    {
        base.Awake();

        gameOver = false;
        allowGrab = false;
        roundActive = false;
        playerTimes = new Dictionary<PlayerNumber, float>();
    }

    new void Start()
    {
        base.Start();
        winningPlayerText.text = "";
        StartRound();
    }

    new void Update()
    {
        base.Update();

        if (!roundActive)
        {
            StartRound();
        }

        if (playerTimes.Count == numPlayers)
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

    void StartRound()
    {
        Debug.Log("Round started");
        roundActive = true;
        reactionSphere.SetAsWaitColor();
        StartCoroutine(WaitForGrab());
    }

    void EndRound()
    {
        float minTime = float.MaxValue;
        PlayerNumber player = PlayerNumber.ONE;
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
        winningPlayerText.text = "Winner: " + player + " time: " + timeTaken;
        playerTimes.Clear();
        reactionSphere.SetAsEndColor();
        StartCoroutine(WaitBetweenRounds());
    }


    IEnumerator WaitForGrab()
    {
        float waitTime = Random.Range(minWait, maxWait);
        Debug.Log("Waiting for " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        allowGrab = true;
        currentTime = Time.timeSinceLevelLoad;
        reactionSphere.SetAsStartColor();
        Debug.Log("Grab allowed");
    }

    void EndGame(PlayerNumber playerNumber)
    {
        Debug.Log("Player " + playerNumber + "has won the game!");
        gameOver = true;
        StartCoroutine(EndGameAfterDelay());
    }

    IEnumerator WaitBetweenRounds()
    {
        yield return new WaitForSeconds(betweenRoundTime);
        roundActive = false;
    }
}
