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
    private RockThrowSpawner[] spawners;

    private new void Start()
    {
        base.Start();

        alivePlayers = (RockPlayer[])players.Clone();
        InitializeSpawners();

        StartCoroutine(StartRoundAfterDelay());
    }

    private new void Update()
    {
        base.Update();

        if (roundActive)
        {
            RockPlayer[] killedPlayers = RemoveKilledPlayers();
            CheckGameOver(killedPlayers);
        }
    }

    private void InitializeSpawners()
    {
        spawners = new RockThrowSpawner[numPlayers];

        float[] pattern = new float[NumRocks];

        for (int i = 0; i < NumRocks; i++)
        {
            // Generate delays within bounds at specified intervals
            // e.g. If min is 1 and max in 3, we can expect values of [1, 1.5, 2, 2.5, 3]
            // with an interval of 0.5f.
            float delay = Mathf.Floor(Random.Range(0, (MaxSpawnDelay - MinSpawnDelay) / SpawnDelayInterval + 1)) * SpawnDelayInterval + MinSpawnDelay;
            pattern[i] = delay;
        }

        for (int i = 0; i < numPlayers; i++)
        {
            Vector3 playerPos = players[i].transform.position;

            RockThrowSpawner spawner = Instantiate(RockSpawnerPrefab) as RockThrowSpawner;
            spawner.transform.position = new Vector3(playerPos.x, 2, playerPos.z);
            spawner.Initialize(pattern);

            spawners[i] = spawner;
        }
    }

    protected override void StartRound()
    {
        base.StartRound();

        foreach(RockThrowSpawner spawner in spawners)
        {
            spawner.StartSpawn();
        }
    }

    protected override void EndRound()
    {
        base.EndRound();

        foreach (RockThrowSpawner spawner in spawners)
        {
            spawner.StopSpawn();
        }
    }

    private RockPlayer[] RemoveKilledPlayers()
    {
        RockPlayer[] killedPlayers = alivePlayers.Where(player => player.IsDead()).ToArray();
        alivePlayers = players.Where(player => !player.IsDead()).ToArray();

        return killedPlayers;
    }

    private void CheckGameOver(RockPlayer[] killedPlayers)
    {
        if (alivePlayers.Length == 1)
        {
            PlayerNumber[] winners = new PlayerNumber[] { alivePlayers[0].GetPlayerNumber() };
            StartCoroutine(EndGameAfterDelay(winners));
        }
        else if (alivePlayers.Length == 0)
        {
            PlayerNumber[] winners = killedPlayers.Select(player => player.GetPlayerNumber()).ToArray();
            StartCoroutine(EndGameAfterDelay(winners));
        }
    }

}
