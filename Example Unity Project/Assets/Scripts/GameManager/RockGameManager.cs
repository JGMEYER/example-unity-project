using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RockGameManager : GameManager<RockPlayer> {

	[Header("GameObjects")]
	[SerializeField]
	private RockThrowSpawner _rockSpawnerPrefab;
	[SerializeField]
	private Text _victoryText;

	[Header("Gameplay")]
	public int numRocks = 6;
	public float minSpawnDelaySec = 1f;
	public float maxSpawnDelaySec = 1.5f;
	public float spawnDelayInterval = 0.5f;

	private RockPlayer[] _alivePlayers;
	private string _gameSelect = "GameSelect";
	private bool _gameOver = false;
	private List<RockThrowSpawner> _spawners;

	new void Start() {
		base.Start();

		_alivePlayers = (RockPlayer[])_players.Clone();
		InitializeSpawners();
	}

	new void Update() {
		base.Update();

		if (!_gameOver) {
			RockPlayer[] killedPlayers = RemoveKilledPlayers();
			CheckGameOver(killedPlayers);
		}
	}

	void InitializeSpawners() {
		_spawners = new List<RockThrowSpawner>();

		float[] pattern = new float[numRocks];

		for (int i = 0; i < numRocks; i++) {
			// Generate delays within bounds at specified intervals
			// e.g. If min is 1 and max in 3, we can expect values of [1, 1.5, 2, 2.5, 3]
			// with an interval of 0.5f.
			float delay = Mathf.Floor(Random.Range(0, (maxSpawnDelaySec - minSpawnDelaySec) / spawnDelayInterval + 1)) * spawnDelayInterval + minSpawnDelaySec;
			pattern[i] = delay;
		}

		foreach (RockPlayer player in _players) {
			Vector3 playerPos = player.transform.position;

			RockThrowSpawner spawner = Instantiate(_rockSpawnerPrefab) as RockThrowSpawner;
			spawner.transform.position = new Vector3(playerPos.x, 2, playerPos.z);
			spawner.Initialize(pattern);

			_spawners.Add(spawner);
		}
	}

	RockPlayer[] RemoveKilledPlayers() {
		RockPlayer[] killedPlayers = _alivePlayers.Where(player => player.IsDead()).ToArray();
		_alivePlayers = _players.Where(player => !player.IsDead()).ToArray();

		return killedPlayers;
	}

	void CheckGameOver(RockPlayer[] killedPlayers) {
		List<string> winners = new List<string>();

		if (_alivePlayers.Length == 1) {
			_victoryText.text = "WINNER!\n";

			winners.Add(_alivePlayers[0].name);
		}
		else if (_alivePlayers.Length == 0) {
			_victoryText.text = "DRAW!\n";

			foreach (RockPlayer player in killedPlayers) {
				winners.Add(player.name);
			}
		}

		if (winners.Count > 0) {
			_gameOver = true;
			_victoryText.text += string.Join(", ", winners.ToArray());

			foreach (RockThrowSpawner spawner in _spawners) {
				spawner.Stop();
			}

			StartCoroutine(EndGameAfterDelay());
		}
	}

}
