using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class LightMazeGameManager : MonoBehaviour {

	[SerializeField]
	private PlayerControlsController _playerControlsController;
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private GameObject _lightMazeWallPrefab;
	[SerializeField]
	private Text _victoryText;

	public bool cameraMoveEnabled = true;
	public float cameraSpeed = 0.5f;
	public int mapWidth = 14;
	public int mapHeight = 100;
	public int rowSpacing = 2;
	public int minPlatformSize = 3;
	public int gapSize = 3;

    private string _sceneSelect = "MainMenu";
	private GameObject[,] _map;
	private bool _gameOver = false;
	private LightMazePlayer[] _players;
	private LightMazePlayer[] _playersAliveLastFrame;

	void Start () {
		Vector3 cameraPos = _camera.transform.position;
		cameraPos[0] = (float) mapWidth / 2;
		cameraPos[1] = -2;
		_camera.transform.position = cameraPos;

		GenerateMap();
		InitializePlayers();
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(_sceneSelect);
		}

		if (!_gameOver) {
			if (cameraMoveEnabled) {
				_camera.transform.Translate(0, Time.deltaTime * cameraSpeed, 0);
			}

			KillOffscreenPlayers();
			CheckGameOver();
		}
	}

	private void KillOffscreenPlayers() {
		foreach(LightMazePlayer player in _players) {
			if (player.IsVisible() || player.IsDead()) {
				continue;
			}

			Vector3 playerPos = player.transform.position;
			Vector3 cameraPos = _camera.transform.position;

			if (playerPos[1] < cameraPos[1]) {
				bool explode = true;
				player.Kill(explode);
			}
		}
	}

	private void CheckGameOver() {
		LightMazePlayer[] alivePlayers = _players.Where(player => !player.IsDead()).ToArray();
		LightMazePlayer[] deadPlayers = _players.Where(player => player.IsDead()).ToArray();

		if (alivePlayers.Length == 1) {
			_gameOver = true;
			_victoryText.text = string.Format("{0} Wins!", _players[0].name);
			bool explode = false;
			_players[0].Kill(explode);

		} else if (alivePlayers.Length == 0) {
			string[] names = _playersAliveLastFrame.Select(player => player.name).ToArray();
			string victoryText = "DRAW!\n";
			victoryText += string.Join(", ", names);
			_victoryText.text = victoryText;
			_gameOver = true;
		}

		_playersAliveLastFrame = alivePlayers;
	}

	private void AddWall(int row, int col) {
		_map[row, col] = Instantiate(_lightMazeWallPrefab) as GameObject;
		_map[row, col].transform.position = new Vector3(col, row, 0);
	}

	private void RemoveWall(int row, int col) {
		Destroy(_map[row, col].gameObject);
		_map[row, col] = null;
	}

	private void GenerateMap() {
		_map = new GameObject[mapHeight, mapWidth];

		for (int col = 0; col < mapWidth; col++) {
			AddWall(0, col);
		}
		for (int row = 0; row < mapHeight; row++) {
			AddWall(row, 0);
			AddWall(row, mapWidth - 1);
		}

		for (int row = rowSpacing; row < mapHeight; row += rowSpacing) {
			for (int col = 0; col < mapWidth; col += 1) {
				AddWall(row, col);
			}

			AddGaps(3, row, 0, mapWidth - 1);
		}
	}

	private int AddGaps(int remaining, int row, int start, int end) {
		if (remaining == 0 || end - start < gapSize) {
			return 0;
		}

		int split = (int)Random.Range(start, end - 1);
		for (int gap = 0; gap < gapSize; gap++) {
			RemoveWall(row, split + gap);
		}

		bool splitLeft = (Random.value < 0.5);
		if (splitLeft) {
			end = split - (minPlatformSize + gapSize - 1);
		} else {
			start = split + (minPlatformSize + gapSize);
		}

		return AddGaps(remaining - 1, row, start, end) + 1;
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

}
