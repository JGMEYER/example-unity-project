using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSceneController : MonoBehaviour {

	[SerializeField]
	private GameObject _rockSpawnerPrefab;
	[SerializeField]
	private PlayerControlsController _playerControlsController;

	public int numRocks = 6;
	public float minSpawnDelaySec = 1f;
	public float maxSpawnDelaySec = 1.5f;
	public float spawnDelayInterval = 0.5f;

	void Start() {
		InitializePlayers();
		InitializeSpawners();
	}

	private void InitializePlayers() {
		RockPlayer[] players = Object.FindObjectsOfType(typeof(RockPlayer)) as RockPlayer[];

		foreach (RockPlayer player in players) {
			// Using player name here is a hack because I don't know how to get a proper
			// player object from a tag. Make sure the object name matches the config.
			string playerDown = _playerControlsController.cfg[player.name]["Down"].StringValue;
			player.playerKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerDown);
		}
	}

	private void InitializeSpawners() {
		float[] pattern = new float[numRocks];

		for (int i = 0; i < numRocks; i++) {
			// Generate delays within bounds at specified intervals
			// e.g. If min is 1 and max in 3, we can expect values of [1, 1.5, 2, 2.5, 3]
			// with an interval of 0.5f.
			float delay = Mathf.Floor(Random.Range(0, (maxSpawnDelaySec - minSpawnDelaySec) / spawnDelayInterval + 1)) * spawnDelayInterval + minSpawnDelaySec;
			pattern[i] = delay;
		}

		RockPlayer[] players = Object.FindObjectsOfType(typeof(RockPlayer)) as RockPlayer[];

		foreach (RockPlayer player in players) {
			Vector3 playerPos = player.transform.position;
			GameObject spawner = Instantiate(_rockSpawnerPrefab) as GameObject;
			spawner.transform.position = new Vector3(playerPos.x, 2, playerPos.z);
			RockThrowSpawner rockSpawnerScript = spawner.GetComponent<RockThrowSpawner>();
			rockSpawnerScript.Initialize(pattern);
		}
	}

}
