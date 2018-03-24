using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RockGameManager : GameManager<RockPlayer>
{

    [Header("GameObjects")]
    [SerializeField]
    private RockThrowSpawner RockSpawnerPrefab;
    [SerializeField]
    private Text VictoryText;

    [Header("Gameplay")]
    public int NumRocks = 6;
    [Tooltip("seconds")]
    public float MinSpawnDelay = 1f;
    [Tooltip("seconds")]
    public float MaxSpawnDelay = 1.5f;
    public float SpawnDelayInterval = 0.5f;

    private RockPlayer[] alivePlayers;
    private bool gameOver = false;
    private List<RockThrowSpawner> spawners;

    new void Start()
    {
        base.Start();

        alivePlayers = (RockPlayer[])players.Clone();
        InitializeSpawners();
    }

    new void Update()
    {
        base.Update();

        if (!gameOver)
        {
            RockPlayer[] killedPlayers = RemoveKilledPlayers();
            CheckGameOver(killedPlayers);
        }
    }

    void InitializeSpawners()
    {
        spawners = new List<RockThrowSpawner>();

        float[] pattern = new float[NumRocks];

        for (int i = 0; i < NumRocks; i++)
        {
            // Generate delays within bounds at specified intervals
            // e.g. If min is 1 and max in 3, we can expect values of [1, 1.5, 2, 2.5, 3]
            // with an interval of 0.5f.
            float delay = Mathf.Floor(Random.Range(0, (MaxSpawnDelay - MinSpawnDelay) / SpawnDelayInterval + 1)) * SpawnDelayInterval + MinSpawnDelay;
            pattern[i] = delay;
        }

        foreach (RockPlayer player in players)
        {
            Vector3 playerPos = player.transform.position;

            RockThrowSpawner spawner = Instantiate(RockSpawnerPrefab) as RockThrowSpawner;
            spawner.transform.position = new Vector3(playerPos.x, 2, playerPos.z);
            spawner.Initialize(pattern);

            spawners.Add(spawner);
        }
    }

    RockPlayer[] RemoveKilledPlayers()
    {
        RockPlayer[] killedPlayers = alivePlayers.Where(player => player.IsDead()).ToArray();
        alivePlayers = players.Where(player => !player.IsDead()).ToArray();

        return killedPlayers;
    }

    void CheckGameOver(RockPlayer[] killedPlayers)
    {
        List<string> winners = new List<string>();

        if (alivePlayers.Length == 1)
        {
            VictoryText.text = "WINNER!\n";
            winners.Add(alivePlayers[0].name);
        }
        else if (alivePlayers.Length == 0)
        {
            VictoryText.text = "DRAW!\n";
            foreach (RockPlayer player in killedPlayers)
            {
                winners.Add(player.name);
            }
        }

        if (winners.Count > 0)
        {
            gameOver = true;
            VictoryText.text += string.Join(", ", winners.ToArray());

            foreach (RockThrowSpawner spawner in spawners)
            {
                spawner.Stop();
            }

            StartCoroutine(EndGameAfterDelay());
        }
    }

}
