using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactionGameManager : GameManager<ReactionPlayer>
{

    [SerializeField]
    private ReactionSphere reactionSphere;
    [SerializeField]
    private Text roundVictoryText;
    [SerializeField]
    private ReactionToast reactionToast;
    [SerializeField]
    private ReactionToast burntToast;

    public int NumPlayers;
    public float MinWait;
    public float MaxWait;
    public float BetweenRoundTime;
    public int NumRounds;

    [Range(0.0f, 100.0f)]
    public float percentBurntToast;

    private bool allowGrab;
    private Dictionary<PlayerNumber, float> playerTimes;
    private float currentTime;
    private int roundsPlayed;

    private new void Awake()
    {
        base.Awake();

        allowGrab = false;
        playerTimes = new Dictionary<PlayerNumber, float>();
    }

    private new void Start()
    {
        base.Start();
        
        roundVictoryText.text = "";
        StartCoroutine(StartRoundAfterDelay());
    }

    private new void Update()
    {
        base.Update();

        if (roundsPlayed >= NumRounds)
        {
            EndGame();
        }

        if (playerTimes.Count == NumPlayers)
        {
            allowGrab = false;
            StartCoroutine(ResetRoundAfterDelay());
        }
    }

    public bool Grab(PlayerNumber playerNumber, float timePressed)
    {
        if (allowGrab && (!playerTimes.ContainsKey(playerNumber)))
        {
            Debug.Log("Grab! " + playerNumber);
            playerTimes.Add(playerNumber, timePressed);
            return true;
        }
        return false;
    }

    protected override void StartRound()
    {
        base.StartRound();

        reactionSphere.SetAsWaitColor();
        StartCoroutine(WaitForGrab());
    }

    protected override void EndRound()
    {
        base.EndRound();

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
        roundVictoryText.text = "Winner: " + player + " time: " + timeTaken;

        foreach (ReactionPlayer rPlayer in players)
        {
            if (rPlayer.GetPlayerNumber().Equals(player))
            {
                rPlayer.IncreaseScore();
            }
        }

        playerTimes.Clear();
        reactionSphere.SetAsEndColor();
        roundsPlayed++;
    }

    protected override void ResetRound()
    {
        base.ResetRound();

        burntToast.resetToast();
        reactionToast.resetToast();

        foreach (ReactionPlayer player in players) 
        {
            player.ResetPlayerPosition();
        }
    }

    private IEnumerator WaitForGrab()
    {
        float waitTime = Random.Range(MinWait, MaxWait);
        Debug.Log("Waiting for " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        float randToast = Random.Range(0, 100);
        if (randToast >= percentBurntToast)
        {
            ThrowToast();
        } 
        else
        {
            ThrowBurntToast();
        }
    }

    private void ThrowToast()
    {
        allowGrab = true;
        currentTime = Time.timeSinceLevelLoad;
        reactionSphere.SetAsStartColor();
        reactionToast.flingToast();
        FindObjectOfType<AudioManager>().Play("Impact");
        Debug.Log("Grab allowed");
    }

    private void ThrowBurntToast()
    {
        burntToast.flingToast();
        FindObjectOfType<AudioManager>().Play("Impact");
        StartCoroutine(ResetAfterBurntToast());
    }

    private IEnumerator ResetAfterBurntToast()
    {
        yield return new WaitForSeconds(BetweenRoundTime);
        ResetRound();
        StartCoroutine(WaitForGrab());
    }

    private void EndGame()
    {
        PlayerNumber winningPlayer = PlayerNumber.One;
        int highestScore = int.MinValue;
        foreach (ReactionPlayer player in players)
        {
            int score = player.GetScore();
            if (score > highestScore)
            {
                highestScore = score;
                winningPlayer = player.GetPlayerNumber();
            }
        }
        allowGrab = false;
        base.EndGame(new PlayerNumber[] { winningPlayer });
    }



}
