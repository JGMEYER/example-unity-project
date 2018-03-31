using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class LightMazeGameManager : GameManager<LightMazePlayer>
{

    [Header("GameObjects")]
    [SerializeField]
    private LightMazeMap map;

    [Header("Debug Settings")]
    public bool AutoScrollEnabled;
    [Range(0f, 10f)]
    public float AutoScrollSpeed = 5f;
    public bool WinConditionsEnabled = true;

    [Header("Map Movement")]
    public bool ScrollMapWhenPlayerAhead = true;
    public float PauseBetweenMapShifts = 1f;

    private float mapShiftPauseCounter = 0f;
    private float mapShiftDistanceRemaining = 0f;

    private new void Start()
    {
        base.Start();

        StartCoroutine(StartRoundAfterDelay());
    }

    private new void Update()
    {
        base.Update();

        if (roundActive)
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
        if (roundActive && AutoScrollEnabled)
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
        LightMazePlayer jetpackPlayer = players.Where(player => player.HasJetpack()).SingleOrDefault();

        PlayerNumber[] winners = null;

        if (jetpackPlayer != null)
        {
            winners = new PlayerNumber[] { jetpackPlayer.GetPlayerNumber() };
        }
        else if (alivePlayers.Length == 1)
        {
            winners = alivePlayers.Select(player => player.GetPlayerNumber()).ToArray();
        }
        else if (alivePlayers.Length == 0)
        {
            winners = playersKilled.Select(player => player.GetPlayerNumber()).ToArray();
        }

        if (winners != null)
        {
            StartCoroutine(EndGameAfterDelay(winners));
        }
    }

}
