using UnityEngine;
using UnityEngine.UI;

public class IceGameManager : GameManager<IcePlayer>
{

    private int playersDead = 0;

    private new void Start()
    {
        base.Start();

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
