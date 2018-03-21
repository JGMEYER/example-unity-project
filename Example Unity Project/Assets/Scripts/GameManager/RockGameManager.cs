using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RockGameManager : MonoBehaviour {

	[Header("GameObjects")]
	[SerializeField]
	private RockThrowSpawner _rockSpawnerPrefab;
	[SerializeField]
	private DEPRECATEDPlayerControlsController _playerControlsController;
	[SerializeField]
	private Text _victoryText;

	[Header("Gameplay")]
	public int numRocks = 6;
	public float minSpawnDelaySec = 1f;
	public float maxSpawnDelaySec = 1.5f;
	public float spawnDelayInterval = 0.5f;

	private string _gameSelect = "GameSelect";
	private bool _gameOver = false;
	private List<RockPlayer> _players;
	private List<RockThrowSpawner> _spawners;

	void Start() {
		InitializePlayers();
		InitializeSpawners();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(_gameSelect);
		}

		if (!_gameOver) {
			List<RockPlayer> removedPlayers = RemoveDeadPlayers();
			CheckGameOver(removedPlayers);
		}
	}

	void InitializePlayers() {
		_players = (FindObjectsOfType(typeof(RockPlayer)) as RockPlayer[]).ToList();

		foreach (RockPlayer player in _players) {
			// Using player name here is a hack because I don't know how to get a proper
			// player object from a tag. Make sure the object name matches the config.
			string playerDown = _playerControlsController.cfg[player.name]["Down"].StringValue;
			player.playerKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerDown);
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

	List<RockPlayer> RemoveDeadPlayers() {
		List<RockPlayer> alivePlayers = _players.Where(player => !player.IsDead()).ToList();
		List<RockPlayer> deadPlayers = _players.Where(player => player.IsDead()).ToList();

		_players = alivePlayers;

		return deadPlayers;
	}

	void CheckGameOver(List<RockPlayer> removedPlayers) {
		List<RockPlayer> alivePlayers = _players;
		List<string> winners = new List<string>();

		if (alivePlayers.Count == 1) {
			_victoryText.text = "WINNER!\n";
			winners.Add(alivePlayers[0].name);
		} else if (alivePlayers.Count == 0) {
			_victoryText.text = "DRAW!\n";
			foreach (RockPlayer player in removedPlayers) {
				winners.Add(player.name);
			}
		}

		if (winners.Count > 0) {
			_gameOver = true;

			foreach (RockThrowSpawner spawner in _spawners) {
				spawner.Stop();
			}

			_victoryText.text += string.Join(", ", winners.ToArray());
		}
	}

}
