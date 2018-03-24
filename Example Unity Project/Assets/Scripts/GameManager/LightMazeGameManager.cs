using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class LightMazeGameManager : GameManager<LightMazePlayer>
{

    [Header("GameObjects")]
    [SerializeField]
    private LightMazeMap map;
    [SerializeField]
    private Text victoryText;

    [Header("Debug Settings")]
    public bool AutoScrollEnabled;
    [Range(0f, 10f)]
    public float AutoScrollSpeed = 5f;
    public bool WinConditionsEnabled = true;

    [Header("Map Movement")]
    public bool ScrollMapWhenPlayerAhead = true;
    public float PauseBetweenMapShifts = 1f;

    private bool gameOver;
    private float mapShiftPauseCounter = 0f;
    private float mapShiftDistanceRemaining = 0f;

    private new void Update()
    {
        base.Update();

        if (!gameOver)
        {
            if (ScrollMapWhenPlayerAhead)
            {
                ScrollMapIfPlayerAhead();
            }

            List<LightMazePlayer> playersKilled = KillFallenPlayers();

            if (WinConditionsEnabled)
            {
                CheckGameOver(playersKilled);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!gameOver && AutoScrollEnabled)
        {
            ScrollRows(AutoScrollSpeed * Time.deltaTime);
        }
    }

    private void ScrollRows(float changeY)
    {
        foreach (LightMazePlayer player in players)
        {
            player.transform.Translate(0, -1 * changeY, 0, Space.World);
        }
        map.ScrollRows(changeY);
    }

    private void ScrollMapIfPlayerAhead()
    {
        mapShiftPauseCounter -= Time.deltaTime;

        bool bump = false;

        if (mapShiftPauseCounter <= 0f)
        {
            foreach (LightMazePlayer player in players)
            {
                bump |= player.transform.position.y > map.MaxAllowedPlayerHeight;
            }
        }

        if (bump)
        {
            mapShiftDistanceRemaining = 1.5f + map.RowSpacing;  // arbitrary
            mapShiftPauseCounter = PauseBetweenMapShifts;
        }

        if (mapShiftDistanceRemaining > 0f)
        {
            float changeY = mapShiftDistanceRemaining * Time.deltaTime;
            ScrollRows(changeY);
            mapShiftDistanceRemaining -= changeY;
        }
    }

    private List<LightMazePlayer> KillFallenPlayers()
    {
        List<LightMazePlayer> playersKilled = new List<LightMazePlayer>();

        foreach (LightMazePlayer player in players)
        {
            if (!player.IsDead() && player.transform.position.y < map.MinAllowedPlayerHeight)
            {
                player.Kill(explode: true);
                playersKilled.Add(player);
            }
        }

        return playersKilled;
    }

    private void CheckGameOver(List<LightMazePlayer> playersKilled)
    {
        LightMazePlayer[] alivePlayers = players.Where(player => !player.IsDead()).ToArray();
        LightMazePlayer[] jetpackPlayers = players.Where(player => player.HasJetpack()).ToArray();

        List<string> winnerNames = new List<string>();
        bool jetpackWin = false;

        if (jetpackPlayers.Length == 1)
        {
            winnerNames.Add(jetpackPlayers[0].name);
            jetpackWin = true;
        }
        else if (alivePlayers.Length == 1)
        {
            winnerNames.Add(alivePlayers[0].name);
        }
        else if (alivePlayers.Length == 0)
        {
            foreach (LightMazePlayer player in playersKilled)
            {
                winnerNames.Add(player.name);
            }
        }

        if (winnerNames.Count > 0)
        {
            GameOver(winnerNames, jetpackWin);
        }
    }

    private void GameOver(List<string> winnerNames, bool jetpackWin)
    {
        gameOver = true;

        if (winnerNames.Count == 1)
        {
            victoryText.text = "WINNER!\n";
        }
        else
        {
            victoryText.text = "DRAW!\n";
        }
        victoryText.text += string.Join(", ", winnerNames.ToArray());

        // TODO jetpackWin: scroll map quickly without interruption and allow players to explode

        StartCoroutine(EndGameAfterDelay());
    }

}
