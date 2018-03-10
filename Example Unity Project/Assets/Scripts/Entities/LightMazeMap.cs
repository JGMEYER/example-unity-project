using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightMazeMap : MonoBehaviour {

	[Header("Prefabs")]
	[SerializeField]
	private GameObject _lightMazeRowPrefab;
	[SerializeField]
	private GameObject _lightMazeWallPrefab;
	[SerializeField]
	private GameObject _lightMazePlatformPrefab;
	[SerializeField]
	private GameObject _lightMazePistonPrefab;
	[SerializeField]
	private GameObject _lightMazeHatchPrefab;
	[SerializeField]
	private GameObject _lightMazeJetpackPrefab;

	[Header("Map Attributes")]
	public int mapWidth = 14;
	public int mapHeight = 10;
	public float minAllowedPlayerHeight = -3f;
	public float maxAllowedPlayerHeight = 14f;

	[Header("Row Attributes")]
	public int totalRows = 20;
	public float rowSpacing = 2.5f;
	public int maxGaps = 3;
	public int gapSize = 3;
	public int minPlatformSize = 3;

	[Header("Environmental Obstacles")]
	public bool spawnPistonsOnPlatformEdges = false;
	[Range(0f, 1f)]
	public float pistonSpawnChance = 0.2f;
	[Range(0f, 1f)]
	public float hatchSpawnChance = 0.2f;

	private Queue<GameObject> _rows = new Queue<GameObject>();
	private int _remainingRows;

	public void Start() {
		if (mapHeight / rowSpacing >= totalRows) {
			throw new System.ArgumentException("(mapHeight / rowSpacing) must be < totalRows");
		}

		_remainingRows = totalRows;

		for (float y = 0; y < mapHeight; y+=rowSpacing) {
			int gaps = (y == 0) ? 0 : maxGaps;
			GameObject row = AddRow(y, gaps);
			_rows.Enqueue(row);
			_remainingRows--;
		}
	}

	public void ScrollRows(float changeY) {
		bool addNewRow = false;
		
		foreach(GameObject row in _rows.ToList()) {
			row.transform.Translate(0, -1 * changeY, 0, Space.World);

			if (row.transform.position.y < minAllowedPlayerHeight - 0.5f) {
				_rows.Dequeue();
				Destroy(row);
				addNewRow = true;
			}
		}

		if (addNewRow && _remainingRows > 0) {
			float lastY = _rows.Last().transform.position.y;
			GameObject row;

			if (_remainingRows > 1) {
				row = AddRow(lastY + rowSpacing, maxGaps);
			} else {
				row = AddJetpackRow(lastY + rowSpacing);
			}

			_rows.Enqueue(row);
			_remainingRows--;
		}
	}

	GameObject AddWall(float x, float y) {
		GameObject wall = Instantiate(_lightMazeWallPrefab);

		wall.transform.localPosition = new Vector3(x, y - (rowSpacing / 2) + 0.5f, 0);
		wall.transform.localScale = new Vector3(1, rowSpacing, 1);

		return wall;
	}

	GameObject AddPlatform(float x, float y, int width = 1, int height = 1) {
		GameObject platform = Instantiate(_lightMazePlatformPrefab);

		platform.transform.localPosition = new Vector3(x, y, 0);
		platform.transform.localScale = new Vector3(width, height, 1);

		return platform;
	}

	GameObject AddRow(float y, int gaps) {
		GameObject row = Instantiate(_lightMazeRowPrefab);
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
			GameObject platform = AddPlatform(platformX, y, width: platformWidth);
			platform.transform.parent = row.transform;
		}

		if (prevRowMap != null && y > rowSpacing) {  // Not first row
			AddEnvironmentObjects(row, prevRow, rowMap, prevRowMap);
		}

		row.GetComponent<LightMazeRowData>().rowMap = rowMap;

		GameObject leftWall = AddWall(-1, y);
		GameObject rightWall = AddWall(mapWidth, y);
		leftWall.transform.parent = row.transform;
		rightWall.transform.parent = row.transform;

		return row;
	}

	GameObject AddJetpackRow(float y) {
		GameObject row = Instantiate(_lightMazeRowPrefab) as GameObject;
		BitArray rowMap = new BitArray(mapWidth, false);

		row.transform.position = new Vector3(0, y, 0);

		AddJetpack(row, (float)mapWidth / 2);

		row.GetComponent<LightMazeRowData>().rowMap = rowMap;

		return row;
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

	void AddPiston(GameObject row, float x) {
		float maxHeight = rowSpacing - (0.5f * 2);
		float y = row.transform.position.y + (maxHeight / 2) + 0.5f;

		GameObject piston = Instantiate(_lightMazePistonPrefab);
		piston.GetComponent<LightMazePiston>().maxHeight = maxHeight;
		piston.transform.localPosition = new Vector3(x, y, 0);
		piston.transform.parent = row.transform;
	}

	void AddHatch(GameObject row, float x, float width) {
		float y = row.transform.position.y;

		GameObject hatch = Instantiate(_lightMazeHatchPrefab);
		hatch.transform.localPosition = new Vector3(x + (width / 2) - 0.5f, y, 0);
		hatch.transform.parent = row.transform;

		Vector3 scale = hatch.transform.localScale;
		scale.x = width;
		hatch.transform.localScale = scale;
	}

	void AddJetpack(GameObject row, float x) {
		float y = row.transform.position.y;

		GameObject jetpack = Instantiate(_lightMazeJetpackPrefab);
		jetpack.transform.localPosition = new Vector3(x, y, 0);
		jetpack.transform.parent = row.transform;
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
			AddPiston(prevRow, randomX);
		}

		List<int[]> gapTuples = GetRowMapAsTuples(rowMap, inverse: true);

		if (gapTuples.Count > 1 && Random.value <= hatchSpawnChance) {
			int[] randomGap = gapTuples[Random.Range(0, gapTuples.Count)];
			int gapStart = randomGap[0];
			int gapWidth = randomGap[1];
			AddHatch(row, gapStart, gapWidth);
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

}
