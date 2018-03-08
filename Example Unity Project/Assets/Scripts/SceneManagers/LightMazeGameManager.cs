using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class LightMazeGameManager : MonoBehaviour {

	[Header("GameObjects")]
	[SerializeField]
	private PlayerControlsController _playerControlsController;
	[SerializeField]
	private LightMazeMap _map;
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private Text _victoryText;

	[Header("Debug Settings")]
	public bool scrollEnabled = false;
	[Range(0f, 10f)]
	public float rowScrollSpeed = 5f;
	public bool winConditionsEnabled = true;

	[Header("Map Movement")]
	public bool scrollMapWhenPlayerAhead = true;
	public float pauseBetweenMapShifts = 1f;

	private string _gameSelect = "GameSelect";
	private bool _gameOver = false;
	private LightMazePlayer[] _players;
	private float _mapShiftPauseCounter = 0f;
	private float _mapShiftDistanceRemaining = 0f;

	private void Start() {
		Vector3 cameraPos = _camera.transform.position;
		_camera.transform.position = new Vector3((float)_map.mapWidth / 2 - 0.5f, cameraPos.y, cameraPos.z);

		InitializePlayers();
	}

	void Update() {
		DoInput();
		if (!_gameOver) {
			if (scrollMapWhenPlayerAhead) {
				ScrollMapIfPlayerAhead(Time.deltaTime);
			}

			List<LightMazePlayer> playersKilled = KillFallenPlayers();

			if (winConditionsEnabled) {
				CheckGameOver(playersKilled);
			}
		}
	}

	void FixedUpdate() {
		if (!_gameOver && scrollEnabled) {
			ScrollRows(rowScrollSpeed * Time.deltaTime);
		}
	}

	void DoInput() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(_gameSelect);
		}
	}

	private void InitializePlayers() {
		LightMazePlayer[] players = Object.FindObjectsOfType(typeof(LightMazePlayer)) as LightMazePlayer[];
		_players = players;

		// TODO FIX!
		foreach (LightMazePlayer player in players) {
			// Using player name here is a hack because I don't know how to get a proper
			// player object from a tag. Make sure the object name matches the config.
			string playerUp = _playerControlsController.cfg[player.name]["Up"].StringValue;
			string playerLeft = _playerControlsController.cfg[player.name]["Left"].StringValue;
			string playerRight = _playerControlsController.cfg[player.name]["Right"].StringValue;
			player.upKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerUp);
			player.leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerLeft);
			player.rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), playerRight);
		}
	}

	void ScrollRows(float changeY) {
		foreach(LightMazePlayer player in _players) {
			player.transform.Translate(0, -1 * changeY, 0, Space.World);
		}
		_map.ScrollRows(changeY);
	}

	void ScrollMapIfPlayerAhead(float deltaTime) {
		_mapShiftPauseCounter -= Time.deltaTime;

		bool bump = false;

		if (_mapShiftPauseCounter <= 0f) {
			foreach (LightMazePlayer player in _players) {
				if (player.transform.position.y > _map.maxAllowedPlayerHeight) {
					bump = true;
				}
			}
		}

		if (bump) {
			_mapShiftDistanceRemaining = 1.5f + _map.rowSpacing;  // arbitrary
			_mapShiftPauseCounter = pauseBetweenMapShifts;
		}

		if (_mapShiftDistanceRemaining > 0f) {
			float changeY = _mapShiftDistanceRemaining * Time.deltaTime;
			ScrollRows(changeY);
			_mapShiftDistanceRemaining -= changeY;
		}
	}

	List<LightMazePlayer> KillFallenPlayers() {
		List<LightMazePlayer> playersKilled = new List<LightMazePlayer>();

		foreach (LightMazePlayer player in _players) {
			if (!player.IsDead() && player.transform.position.y < _map.minAllowedPlayerHeight) {
				player.Kill(explode: true);
				playersKilled.Add(player);
			}
		}

		return playersKilled;
	}

	void CheckGameOver(List<LightMazePlayer> playersKilled) {
		LightMazePlayer[] alivePlayers = _players.Where(player => !player.IsDead()).ToArray();
		LightMazePlayer[] jetpackPlayers = _players.Where(player => player.HasJetpack()).ToArray();

		List<string> winnerNames = new List<string>();
		bool jetpackWin = false;

		if (jetpackPlayers.Length == 1) {
			winnerNames.Add(jetpackPlayers[0].name);
			jetpackWin = true;
		} else if (alivePlayers.Length == 1) {
			winnerNames.Add(alivePlayers[0].name);
		} else if (alivePlayers.Length == 0) {
			foreach (LightMazePlayer player in playersKilled) {
				winnerNames.Add(player.name);
			}
		}

		if (winnerNames.Count > 0) {
			StartCoroutine(GameOver(winnerNames, jetpackWin));
		}
	}

	IEnumerator GameOver(List<string> winnerNames, bool jetpackWin) {
		_gameOver = true;

		if (winnerNames.Count == 1) {
			_victoryText.text = "WINNER!\n";
		} else {
			_victoryText.text = "DRAW!\n";
		}
		_victoryText.text += string.Join(", ", winnerNames.ToArray());

		// TODO jetpackWin: scroll map quickly without interruption and allow players to explode

		yield return new WaitForSeconds(3f);

		SceneManager.LoadSceneAsync(_gameSelect);
	}

}
