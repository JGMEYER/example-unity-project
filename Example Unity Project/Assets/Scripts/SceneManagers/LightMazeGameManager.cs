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
	private GameObject _lightMazeRowPrefab;
	[SerializeField]
	private GameObject _lightMazePistonPrefab;
	[SerializeField]
	private GameObject _lightMazeHatchPrefab;
	[SerializeField]
	private Text _victoryText;

	public bool scrollEnabled = true;
	public bool winConditionsEnabled = true;
	public bool shiftMapWhenPlayerAhead = true;
	[Range(0f, 10f)]
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
	[Range(0f, 1f)]
	public float pistonSpawnChance = 0.2f;
	public bool spawnPistonsOnPlatformEdges = false;
	[Range(0f, 1f)]
	public float hatchSpawnChance = 0.2f;

    private string _gameSelect = "GameSelect";
	private bool _gameOver = false;
	private Queue<GameObject> _rows = new Queue<GameObject>();
	private LightMazePlayer[] _players;
	private float _mapShiftPauseCounter = 0f;
	private float _mapShiftDistanceRemaining = 0f;

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
			ScrollRows(rowScrollSpeed * Time.deltaTime);
		}
	}

	void DoInput() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(_gameSelect);
		}
	}

	void ScrollRows(float changeY) {
		bool addNewRow = false;

		foreach(LightMazePlayer player in _players) {
			player.transform.Translate(0, -1 * changeY, 0, Space.World);
		}

		foreach(GameObject row in _rows.ToList()) {
			row.transform.Translate(0, -1 * changeY, 0, Space.World);

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
		_mapShiftPauseCounter -= Time.deltaTime;

		bool bump = false;

		if (_mapShiftPauseCounter <= 0f) {
			foreach (LightMazePlayer player in _players) {
				if (player.transform.position.y > maxAllowedPlayerHeight) {
					bump = true;
				}
			}
		}

		if (bump) {
			_mapShiftDistanceRemaining = 1.5f + rowSpacing;  // arbitrary
			_mapShiftPauseCounter = pauseBetweenMapShifts;
		}

		if (_mapShiftDistanceRemaining > 0f) {
			float changeY = _mapShiftDistanceRemaining * Time.deltaTime;
			ScrollRows(changeY);
			_mapShiftDistanceRemaining -= changeY;
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
		GameObject row = Instantiate(_lightMazeRowPrefab) as GameObject;
		GameObject prevRow = null;
		BitArray rowMap = new BitArray(mapWidth, true);
		BitArray prevRowMap = null;

		if (_rows.Count > 0) {
			prevRow = _rows.Last();
			prevRowMap = prevRow.GetComponent<LightMazeRowData>().rowMap;
		}

		if (prevRowMap == null) {
			CreateGaps(rowMap, gaps, 0, rowMap.Length - 1);
		} else { // Do not add gaps where last row had gaps prior
			int gapsAdded = 0;
			int gapsRemaining = gaps;
			List<int[]> prevPlatformTuples = GetRowMapAsTuples(prevRowMap);

			while (prevPlatformTuples.Count > 0 && gapsRemaining > 0) {
				int randIndex = Random.Range(0, prevPlatformTuples.Count);
				int[] randPlatform = prevPlatformTuples[randIndex];
				prevPlatformTuples.RemoveAt(randIndex);

				int platformStart = randPlatform[0];
				int platformWidth = randPlatform[1];
				int platformEnd = platformStart + platformWidth - 1;

				gapsAdded = CreateGaps(rowMap, gapsRemaining, platformStart, platformEnd);
				gapsRemaining -= gapsAdded;
			}
		}

		row.transform.position = new Vector3(0, y, 0);

		foreach (int[] platformTuple in GetRowMapAsTuples(rowMap)) {
			int platformStart = platformTuple[0];
			int platformWidth = platformTuple[1];

			float platformX = platformStart + ((float)platformWidth / 2) - 0.5f;
			GameObject platform = AddPlatform(platformX, y, width: platformWidth, local: true);
			platform.AddComponent<CarryRigidBodies>();
			platform.transform.parent = row.transform;
		}

		if (prevRowMap != null && y > rowSpacing) {  // not first row
			AddEnvironmentObjects(row, prevRow, rowMap, prevRowMap);
		}

		row.GetComponent<LightMazeRowData>().rowMap = rowMap;

		return row;
	}

	GameObject AddPiston(float x, float platformY) {
		GameObject piston = Instantiate(_lightMazePistonPrefab) as GameObject;

		float maxHeight = rowSpacing - (0.5f * 2);
		piston.GetComponent<LightMazePiston>().maxHeight = maxHeight;
		piston.transform.localPosition = new Vector3(x, platformY + (maxHeight / 2) + 0.5f, 0);

		return piston;
	}

	GameObject AddHatch(float x, float width, float platformY) {
		GameObject hatch = Instantiate(_lightMazeHatchPrefab) as GameObject;

		hatch.transform.localPosition = new Vector3(x + width / 2 - 0.5f, platformY, 0);

		Vector3 scale = hatch.transform.localScale;
		scale.x = width;
		hatch.transform.localScale = scale;

		return hatch;
	}

	void AddEnvironmentObjects(GameObject row, GameObject prevRow, BitArray rowMap, BitArray prevRowMap) {
		BitArray matching = new BitArray(rowMap).And(prevRowMap);

		int enclosedLeftEnd = -1;
		int enclosedRightStart = matching.Count;

		// Two platforms are considered enclosed if they form a 3 sided figure with a wall
		for (int x = 0; x < matching.Count; x++) {
			if (matching[x]) {
				enclosedLeftEnd++;	
			} else {
				break;
			}
		}
		for (int x = matching.Count - 1; x >= 0; x--) {
			if (matching[x]) {
				enclosedRightStart--;	
			} else {
				break;
			}
		}

		List<int> pistonOptions = new List<int>();

		for (int x = 1; x < matching.Count - 1; x++) {
			bool isOnEdge = !matching[x - 1] || !matching[x + 1];

			if (matching[x] && x > enclosedLeftEnd && x < enclosedRightStart) {
				if (!isOnEdge || spawnPistonsOnPlatformEdges) {
					pistonOptions.Add(x);
				}
			}
		}

		if (pistonOptions.Count > 0 && Random.value <= pistonSpawnChance) {
			int randomX = pistonOptions[Random.Range(0, pistonOptions.Count)];
			GameObject piston = AddPiston(randomX, prevRow.transform.position.y);
			piston.transform.parent = prevRow.transform;
		}

		List<int[]> gapTuples = GetRowMapAsTuples(rowMap, inverse: true);

		if (gapTuples.Count > 1 && Random.value <= hatchSpawnChance) {
			int[] randomGap = gapTuples[Random.Range(0, gapTuples.Count)];
			int gapStart = randomGap[0];
			int gapWidth = randomGap[1];
			GameObject hatch = AddHatch(gapStart, gapWidth, row.transform.position.y);
			hatch.transform.parent = row.transform;
		}

		foreach (int[] gapTuple in gapTuples) {
			int gapStart = gapTuple[0];
			int gapWidth = gapTuple[1];
			

		}
	}

	List<int[]> GetRowMapAsTuples(BitArray rowMap, bool inverse = false) {
		List<int[]> tuples = new List<int[]>();
		int segmentStart = 0;
		int segmentWidth = 0;

		for (int x = 0; x < rowMap.Length; x++) {
			bool include = (rowMap.Get(x) && !inverse) || (!rowMap.Get(x) && inverse);

			if (include) {
				segmentWidth += 1;
			}

			if (!include || x == mapWidth - 1) {
				if (segmentWidth > 0) {
					int[] tuple = {segmentStart, segmentWidth};
					tuples.Add(tuple);
				}
				segmentStart = x + 1;
				segmentWidth = 0;
			}
		}

		return tuples;
	}

	int CreateGaps(BitArray rowMap, int remaining, int start, int end) {
		if (remaining == 0 || end - start < gapSize) {
			return 0;
		}

		// TODO why do we do end - 1 here?
		int split = Random.Range(start, end - 1);
		if (end - start + 1 == gapSize) {
			split = 0;
		}

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
