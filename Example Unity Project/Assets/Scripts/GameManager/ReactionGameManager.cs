using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactionGameManager : GameManager<ReactionPlayer>
{

    [SerializeField]
    private ReactionSphere reactionSphere;
    [SerializeField]
    private Text winningPlayerText;
    [SerializeField]
    private ReactionToast reactionToast;
    [SerializeField]
    private ReactionToast burntToast;

    public int NumPlayers;
    public float MinWait;
    public float MaxWait;
    public float BetweenRoundTime;

    private bool allowGrab;
    private Dictionary<PlayerNumber, float> playerTimes;
    private float currentTime;

    private new void Awake()
    {
        base.Awake();

        allowGrab = false;
        playerTimes = new Dictionary<PlayerNumber, float>();
    }

    private new void Start()
    {
        base.Start();
        
        winningPlayerText.text = "";
        StartCoroutine(StartRoundAfterDelay());
    }

    private new void Update()
    {
        base.Update();

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
        winningPlayerText.text = "Winner: " + player + " time: " + timeTaken;
        playerTimes.Clear();
        reactionSphere.SetAsEndColor();
    }

    protected override void ResetRound()
    {
        base.ResetRound();

        burntToast.resetToast();
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
        allowGrab = true;
        currentTime = Time.timeSinceLevelLoad;
        reactionSphere.SetAsStartColor();
        //reactionToast.flingToast();
        burntToast.flingToast();
        FindObjectOfType<AudioManager>().Play("Impact");
        Debug.Log("Grab allowed");
    }

}
