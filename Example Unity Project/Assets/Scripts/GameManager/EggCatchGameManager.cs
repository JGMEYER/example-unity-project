using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCatchGameManager : GameManager<EggCatchPlayer>
{

    [SerializeField]
    private GameObject player1Panel;
    [SerializeField]
    private GameObject player2Panel;
    [SerializeField]
    private GameObject player3Panel;
    [SerializeField]
    private GameObject player4Panel;

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
    }

    private new void Start()
    {
        base.Start();

        StartCoroutine(StartRoundAfterDelay());
    }

    public void AddPoints(PlayerNumber playerNumber, int numPoints)
    {
        Debug.Log("Adding " + numPoints + " to player " + (int)playerNumber);

        points[playerNumber] += numPoints;
        playerPanels[playerNumber].GetComponentInChildren<Text>().text = points[playerNumber] + "";
    }

}
