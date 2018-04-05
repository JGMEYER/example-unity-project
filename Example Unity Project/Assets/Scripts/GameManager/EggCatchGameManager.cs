using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EggCatchGameManager : GameManager<EggCatchPlayer>
{

    [SerializeField]
    private Text gameTimer;
    [SerializeField]
    private GameObject player1Panel;
    [SerializeField]
    private GameObject player2Panel;
    [SerializeField]
    private GameObject player3Panel;
    [SerializeField]
    private GameObject player4Panel;

    [Tooltip("seconds")]
    public float TotalGameTime = 45f;

    private float gameTime;
    private Dictionary<PlayerNumber, int> points;
    private Dictionary<PlayerNumber, GameObject> playerPanels;

    private new void Awake()
    {
        base.Awake();

        gameTime = TotalGameTime;
        points = new Dictionary<PlayerNumber, int>();
        playerPanels = new Dictionary<PlayerNumber, GameObject>();

        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber)))
        {
            if ((int)playerNumber <= numPlayers)
            {
                points.Add(playerNumber, 0);
                switch (playerNumber)
                {
                    case PlayerNumber.One:
                        playerPanels.Add(PlayerNumber.One, player1Panel);
                        break;
                    case PlayerNumber.Two:
                        playerPanels.Add(PlayerNumber.Two, player2Panel);
                        break;
                    case PlayerNumber.Three:
                        playerPanels.Add(PlayerNumber.Three, player3Panel);
                        break;
                    case PlayerNumber.Four:
                        playerPanels.Add(PlayerNumber.Four, player4Panel);
                        break;
                }
            }
            else
            {
                switch (playerNumber)
                {
                    case PlayerNumber.One:
                        Destroy(player1Panel.gameObject);
                        break;
                    case PlayerNumber.Two:
                        Destroy(player2Panel.gameObject);
                        break;
                    case PlayerNumber.Three:
                        Destroy(player3Panel.gameObject);
                        break;
                    case PlayerNumber.Four:
                        Destroy(player4Panel.gameObject);
                        break;
                }
            }
        }

        UpdateGameTimer();
    }

    private new void Start()
    {
        base.Start();

        StartCoroutine(StartRoundAfterDelay());
    }

    private new void Update()
    {
        if (roundActive)
        {
            gameTime -= Time.deltaTime;
            if (gameTime < 0f)
            {
                gameTime = 0f;
                UpdateGameTimer();

                PlayerNumber[] winners = GetWinners();
                StartCoroutine(EndGameAfterDelay(winners));
            }
            else
            {
                UpdateGameTimer();
            }
        }
    }

    private void UpdateGameTimer()
    {
        float min = Mathf.Floor(gameTime / 60f);
        float sec = gameTime % 60f;
        float fraction = ((gameTime * 100) % 100);

        if (TotalGameTime >= 60f)
        {
            gameTimer.text = String.Format("{0:00}:{1:00}:{2:00}", min, sec, fraction);
        }
        else
        {
            // Much more exciting when all the numbers are moving
            gameTimer.text = String.Format("{0:00}:{1:00}", sec, fraction);
        }
    }

    // God have mercy...
    private PlayerNumber[] GetWinners()
    {
        List<PlayerNumber> winners = new List<PlayerNumber>();
        float max = points.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;  // largest point value

        foreach (KeyValuePair<PlayerNumber, int> pair in points)
        {
            if (pair.Value == max)
            {
                winners.Add(pair.Key);
            }
        }

        return winners.ToArray();
    }

    public void AddPoints(PlayerNumber playerNumber, int numPoints)
    {
        if (roundActive)
        {
            points[playerNumber] += numPoints;
            playerPanels[playerNumber].GetComponentInChildren<Text>().text = points[playerNumber] + "";
        }
    }

}
