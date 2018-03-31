using UnityEngine;
using UnityEngine.UI;

public class IceGameManager : GameManager<IcePlayer>
{

    [SerializeField]
    private Text victoryText;

    private int playersDead = 0;

    private new void Start()
    {
        base.Start();

        victoryText.text = "";

        StartCoroutine(StartRoundAfterDelay());
    }

    public void HandlePlayerDeath(string playerName)
    {
        playersDead++;

        if (playersDead >= numPlayers - 1)
        {
            StartCoroutine(EndGameAfterDelay(null));  // really should pass array of winners
        }
    }

}
