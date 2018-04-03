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
    public float GameTime = 45f;

    private Dictionary<PlayerNumber, int> points;
    private Dictionary<PlayerNumber, GameObject> playerPanels;

    private new void Awake()
    {
        base.Awake();

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
            GameTime -= Time.deltaTime;
            if (GameTime < 0f)
            {
                GameTime = 0f;
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
        float min = Mathf.Floor(GameTime / 60f);
        float sec = GameTime % 60f;
        float fraction = ((GameTime * 100) % 100);

        gameTimer.text = String.Format("{0:00}:{1:00}:{2:00}", min, sec, fraction);
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
