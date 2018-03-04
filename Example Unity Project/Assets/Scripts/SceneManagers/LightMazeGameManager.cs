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

	public bool scrollEnabled = true;
	public bool winConditionsEnabled = true;
	public bool shiftMapWhenPlayerAhead = true;
	public float rowScrollSpeed = 0.5f;
	public int mapWidth = 14;
	public int mapHeight = 10;
	public float rowSpacing = 2.5f;
	public int maxGaps = 3;
	public int gapSize = 3;
	public int minPlatformSize = 3;
	public float minAllowedPlayerHeight = -3f;
	public float maxAllowedPlayerHeight = 14f;
	public float pauseBetweenMapShifts = 1f;

    private string _gameSelect = "GameSelect";
	private bool _gameOver = false;
	private Queue<GameObject> _rows = new Queue<GameObject>();
	private LightMazePlayer[] _players;
	private float mapShiftPauseCounter = 0f;

	private void Start() {
		Vector3 cameraPos = _camera.transform.position;
		_camera.transform.position = new Vector3(mapWidth / 2, cameraPos.y, cameraPos.z);

		GenerateStartingMap();
		InitializePlayers();
	}

	void Update() {
		DoInput();
		if (!_gameOver) {
			if (shiftMapWhenPlayerAhead) {
				ShiftMapIfPlayerAhead(Time.deltaTime);
			}
			KillFallenPlayers();
		}
	}

	private void FixedUpdate() {
		if (!_gameOver && scrollEnabled) {
			ScrollRows(rowScrollSpeed);
		}
	}

	void DoInput() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(_gameSelect);
		}
	}

	void ScrollRows(float changeY) {
		bool addNewRow = false;

		foreach(GameObject row in _rows.ToList()) {
			row.transform.Translate(0, -1 * Time.deltaTime * changeY, 0);

			if (row.transform.position.y < minAllowedPlayerHeight - 0.5f) {
				_rows.Dequeue();
				Destroy(row);
				addNewRow = true;
			}
		}

		if (addNewRow) {
			float lastY = _rows.Last().transform.position.y;
			GameObject row = AddRow(lastY + rowSpacing, maxGaps);
			_rows.Enqueue(row);
		}
	}

	void ShiftMapIfPlayerAhead(float deltaTime) {
		mapShiftPauseCounter -= Time.deltaTime;

		if (mapShiftPauseCounter > 0) {
			return;
		}

		bool bump = false;
		foreach (LightMazePlayer player in _players) {
			if (player.transform.position.y > maxAllowedPlayerHeight) {
				bump = true;
			}
		}

		if (bump) {
			ScrollRows(1f + rowSpacing);
			mapShiftPauseCounter = pauseBetweenMapShifts;
		}
	}

	void KillFallenPlayers() {
		List<LightMazePlayer> playersKilled = new List<LightMazePlayer>();

		foreach (LightMazePlayer player in _players) {
			if (!player.IsDead() && player.transform.position.y < minAllowedPlayerHeight) {
				player.Kill(explode: true);
				playersKilled.Add(player);
			}
		}

		LightMazePlayer[] alivePlayers = _players.Where(player => !player.IsDead()).ToArray();

		if (winConditionsEnabled) {
			CheckGameOver(playersKilled.ToArray<LightMazePlayer>(), alivePlayers);
		}
	}

	void CheckGameOver(LightMazePlayer[] playersKilled, LightMazePlayer[] alivePlayers) {
		List<string> winners = new List<string>();

		if (alivePlayers.Length == 1) {
			_victoryText.text = "WINNER!\n";
			winners.Add(alivePlayers[0].name);
		} else if (alivePlayers.Length == 0) {
			_victoryText.text = "DRAW!\n";
			foreach (LightMazePlayer player in playersKilled) {
				winners.Add(player.name);
			}
		}

		if (winners.Count > 0) {
			_gameOver = true;
			_victoryText.text += string.Join(", ", winners.ToArray());
		}
	}

	void GenerateStartingMap() {
		int wallHeight = mapHeight + (int)Mathf.Ceil(Mathf.Abs(minAllowedPlayerHeight));
		int wallY = mapHeight / 2 - (int)Mathf.Ceil(Mathf.Abs(minAllowedPlayerHeight));

		AddPlatform(-1, wallY, height: wallHeight);
		AddPlatform(mapWidth, wallY, height: wallHeight);

		for (float y = 0; y < mapHeight; y+=rowSpacing) {
			int gaps = (y == 0) ? 0 : maxGaps;
			GameObject row = AddRow(y, gaps);
			_rows.Enqueue(row);
		}
	}

	GameObject AddPlatform(float x, float y, int width = 1, int height = 1, bool local = false) {
		GameObject wall = Instantiate(_lightMazeWallPrefab) as GameObject;

		if (local) {
			wall.transform.localPosition = new Vector3(x, y, 0);
		} else {
			wall.transform.position = new Vector3(x, y, 0);
		}

		wall.transform.localScale = new Vector3(width, height, 1);

		return wall;
	}

	GameObject AddRow(float y, int gaps) {
		GameObject row = new GameObject("Row");
		BitArray rowMap = new BitArray(mapWidth, true);

		CreateGaps(rowMap, gaps, 0, rowMap.Length - 1);

		row.transform.position = new Vector3(0, y, 0);

		int wallStart = 0;
		int wallWidth = 0;

		for (int x = 0; x < mapWidth; x++) {
			if (rowMap.Get(x)) {
				wallWidth += 1;
			}

			if (!rowMap.Get(x) || x == mapWidth - 1) {
				if (wallWidth > 0) {
					float wallX = wallStart + ((float)wallWidth / 2) - 0.5f;
					GameObject wall = AddPlatform(wallX, y, width: wallWidth, local: true);
					wall.AddComponent<CarryRigidBodies>();
					wall.transform.parent = row.transform;
				}
				wallStart = x + 1;
				wallWidth = 0;
			}
		}

		return row;
	}

	int CreateGaps(BitArray rowMap, int remaining, int start, int end) {
		if (remaining == 0 || end - start < gapSize) {
			return 0;
		}

		int split = Random.Range(start, end - 1);
		for (int gap = 0; gap < gapSize; gap++) {
			rowMap.Set(split + gap, false);
		}

		bool splitLeft = (Random.value < 0.5);
		if (splitLeft) {
			end = split - (minPlatformSize + gapSize - 1);
		} else {
			start = split + (minPlatformSize + gapSize);
		}

		return CreateGaps(rowMap, remaining - 1, start, end);
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
