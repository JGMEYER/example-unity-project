using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightMazeMap : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField]
    private GameObject lightMazeRowPrefab;
    [SerializeField]
    private GameObject lightMazeWallPrefab;
    [SerializeField]
    private GameObject lightMazePlatformPrefab;
    [SerializeField]
    private GameObject lightMazePistonPrefab;
    [SerializeField]
    private GameObject lightMazeHatchPrefab;
    [SerializeField]
    private GameObject lightMazeJetpackPrefab;

    [Header("Map Attributes")]
    public int MapWidth = 14;
    public int MapHeight = 10;
    public float MinAllowedPlayerHeight = -3f;
    public float MaxAllowedPlayerHeight = 14f;

    [Header("Row Attributes")]
    [SerializeField]
    private int totalRows = 20;
    public float RowSpacing = 2.5f;
    public int MaxGaps = 3;
    public int GapSize = 3;
    public int MinPlatformSize = 3;

    [Header("Environmental Obstacles")]
    public bool SpawnPistonsOnPlatformEdges;
    [Range(0f, 1f)]
    public float PistonSpawnChance = 0.2f;
    [Range(0f, 1f)]
    public float HatchSpawnChance = 0.2f;

    // TODO find better object type with fast front pop and can be indexed
    private Queue<GameObject> rows = new Queue<GameObject>();
    private int remainingRows;
    private bool valuesChanged;

    private void Start()
    {
        remainingRows = totalRows;

        for (float y = 0; y < MapHeight; y += RowSpacing)
        {
            GameObject row = AddRow(y);
            rows.Enqueue(row);
            remainingRows--;
        }
    }

    private void Update()
    {
        if (valuesChanged)
        { // Should be true after Start() to trigger original gen
            RepopulateRows();
            valuesChanged = false;
        }
    }

    private void OnValidate()
    {
        // TODO is there an "assert" equivalent in C# for these checks?

        if (RowSpacing < 1)
        {
            RowSpacing = 1;
            throw new System.ArgumentException("rowSpacing cannot be < 1");
        }
        if (MapHeight / RowSpacing >= totalRows)
        {
            throw new System.ArgumentException("(mapHeight / rowSpacing) must be < totalRows");
        }

        if (GapSize < 1)
        {
            GapSize = 1;
            throw new System.ArgumentException("gapSize cannot be < 1");
        }
        if (GapSize > MapWidth)
        {
            GapSize = MapWidth;
            throw new System.ArgumentException("gapSize cannot be > mapWidth");
        }
        if (MinPlatformSize < 1)
        {
            MinPlatformSize = 1;
            throw new System.ArgumentException("minPlatformSize cannot be < 1");
        }
        if (MinPlatformSize + GapSize > MapWidth)
        {
            throw new System.ArgumentException("(minPlatformSize + gapSize) cannot be > mapWidth");
        }

        valuesChanged = true;
    }

    public void ScrollRows(float changeY)
    {
        bool addNewRow = false;

        foreach (GameObject row in rows.ToList())
        {
            row.transform.Translate(0, -1 * changeY, 0, Space.World);

            if (row.transform.position.y < MinAllowedPlayerHeight - 0.5f)
            {
                rows.Dequeue();
                Destroy(row);
                addNewRow = true;
            }
        }

        if (addNewRow && remainingRows > 0)
        {
            GameObject prevRow = rows.Last();
            float prevY = prevRow.transform.position.y;

            GameObject row = AddRow(prevY + RowSpacing);

            if (remainingRows > 1)
            {
                PopulateRow(row, prevRow, MaxGaps);
            }
            else
            {
                PopulateJetpackRow(row);
            }

            rows.Enqueue(row);
            remainingRows--;
        }
    }

    public void RepopulateRows()
    {
        GameObject[] rowsArray = rows.ToArray();
        GameObject prevRow = null;

        for (int i = 0; i < rowsArray.Length; i++)
        {
            GameObject row = rowsArray[i];
            int gaps = (i == 0) ? 0 : MaxGaps;

            if (prevRow)
            {
                Vector3 newPosition = row.transform.position;
                float newY = prevRow.transform.position.y + RowSpacing;

                newPosition.y = newY;
                row.transform.position = newPosition;
            }

            ClearRow(row);

            if (remainingRows == 0 && i == rowsArray.Length - 1)
            {
                PopulateJetpackRow(row);
            }
            else
            {
                PopulateRow(row, prevRow, gaps);
            }

            prevRow = row;
        }
    }

    private GameObject AddWall(float x, float y)
    {
        GameObject wall = Instantiate(lightMazeWallPrefab);

        wall.transform.localPosition = new Vector3(x, y - (RowSpacing / 2) + 0.5f, 0);
        wall.transform.localScale = new Vector3(1, RowSpacing, 1);

        return wall;
    }

    private GameObject AddPlatform(float x, float y, int width = 1, int height = 1)
    {
        GameObject platform = Instantiate(lightMazePlatformPrefab);

        platform.transform.localPosition = new Vector3(x, y, 0);
        platform.transform.localScale = new Vector3(width, height, 1);

        return platform;
    }

    private GameObject AddRow(float y)
    {
        GameObject row = Instantiate(lightMazeRowPrefab);

        row.transform.position = new Vector3(0, y, 0);
        row.GetComponent<LightMazeRowData>().RowMap = new BitArray(MapWidth, false);

        return row;
    }

    private void ClearRow(GameObject row)
    {
        row.GetComponent<LightMazeRowData>().RowMap = new BitArray(MapWidth, false);

        Transform[] children = row.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (Transform child in children)
        {
            if (child.gameObject != row)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void PopulateRow(GameObject row, GameObject prevRow, int gaps)
    {
        float y = row.transform.position.y;

        BitArray rowMap = row.GetComponent<LightMazeRowData>().RowMap = new BitArray(MapWidth, true);
        BitArray prevRowMap = null;

        if (prevRow == null)
        {
            CreateGaps(rowMap, gaps, 0, rowMap.Length - 1);
            // Do not add gaps where last row had gaps prior
        }
        else
        {
            int gapsAdded = 0;
            int gapsRemaining = gaps;
            prevRowMap = prevRow.GetComponent<LightMazeRowData>().RowMap;
            List<int[]> prevPlatformTuples = GetRowMapAsTuples(prevRowMap);

            while (prevPlatformTuples.Count > 0 && gapsRemaining > 0)
            {
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

        foreach (int[] platformTuple in GetRowMapAsTuples(rowMap))
        {
            int platformStart = platformTuple[0];
            int platformWidth = platformTuple[1];

            float platformX = platformStart + ((float)platformWidth / 2) - 0.5f;
            GameObject platform = AddPlatform(platformX, y, width: platformWidth);
            platform.transform.parent = row.transform;
        }

        if (prevRowMap != null && y > RowSpacing)
        {  // Not first row
            AddEnvironmentObjects(row, prevRow, rowMap, prevRowMap);
        }

        GameObject leftWall = AddWall(-1, y);
        GameObject rightWall = AddWall(MapWidth, y);
        leftWall.transform.parent = row.transform;
        rightWall.transform.parent = row.transform;
    }

    private GameObject PopulateJetpackRow(GameObject row)
    {
        row.GetComponent<LightMazeRowData>().RowMap = new BitArray(MapWidth, false);

        AddJetpack(row, (float)MapWidth / 2);

        return row;
    }

    private int CreateGaps(BitArray rowMap, int remaining, int start, int end)
    {
        if (remaining == 0 || end - start < GapSize)
        {
            return 0;
        }

        // TODO why do we do end - 1 here?
        int split = Random.Range(start, end - 1);
        if (end - start + 1 == GapSize)
        {
            split = 0;
        }

        for (int gap = 0; gap < GapSize; gap++)
        {
            rowMap.Set(split + gap, false);
        }

        bool splitLeft = (Random.value < 0.5);
        if (splitLeft)
        {
            end = split - (MinPlatformSize + GapSize - 1);
        }
        else
        {
            start = split + (MinPlatformSize + GapSize);
        }

        return CreateGaps(rowMap, remaining - 1, start, end);
    }

    private void AddPiston(GameObject row, float x)
    {
        float maxHeight = RowSpacing - (0.5f * 2);
        float y = row.transform.position.y + (maxHeight / 2) + 0.5f;

        GameObject piston = Instantiate(lightMazePistonPrefab);
        piston.GetComponent<LightMazePiston>().MaxHeight = maxHeight;
        piston.transform.localPosition = new Vector3(x, y, 0);
        piston.transform.parent = row.transform;
    }

    private void AddHatch(GameObject row, float x, float width)
    {
        float y = row.transform.position.y;

        GameObject hatch = Instantiate(lightMazeHatchPrefab);
        hatch.transform.localPosition = new Vector3(x + (width / 2) - 0.5f, y, 0);
        hatch.transform.parent = row.transform;

        Vector3 scale = hatch.transform.localScale;
        scale.x = width;
        hatch.transform.localScale = scale;
    }

    private void AddJetpack(GameObject row, float x)
    {
        float y = row.transform.position.y;

        GameObject jetpack = Instantiate(lightMazeJetpackPrefab);
        jetpack.transform.localPosition = new Vector3(x, y, 0);
        jetpack.transform.parent = row.transform;
    }

    private void AddEnvironmentObjects(GameObject row, GameObject prevRow, BitArray rowMap, BitArray prevRowMap)
    {
        BitArray matching = new BitArray(rowMap).And(prevRowMap);

        int enclosedLeftEnd = -1;
        int enclosedRightStart = matching.Count;

        // Two platforms are considered enclosed if they form a 3 sided figure with a wall
        for (int x = 0; x < matching.Count; x++)
        {
            if (matching[x])
            {
                enclosedLeftEnd++;
            }
            else
            {
                break;
            }
        }
        for (int x = matching.Count - 1; x >= 0; x--)
        {
            if (matching[x])
            {
                enclosedRightStart--;
            }
            else
            {
                break;
            }
        }

        List<int> pistonOptions = new List<int>();

        for (int x = 1; x < matching.Count - 1; x++)
        {
            bool isOnEdge = !matching[x - 1] || !matching[x + 1];

            if (matching[x] && x > enclosedLeftEnd && x < enclosedRightStart)
            {
                if (!isOnEdge || SpawnPistonsOnPlatformEdges)
                {
                    pistonOptions.Add(x);
                }
            }
        }

        if (pistonOptions.Count > 0 && Random.value <= PistonSpawnChance)
        {
            int randomX = pistonOptions[Random.Range(0, pistonOptions.Count)];
            AddPiston(prevRow, randomX);
        }

        List<int[]> gapTuples = GetRowMapAsTuples(rowMap, inverse: true);

        if (gapTuples.Count > 1 && Random.value <= HatchSpawnChance)
        {
            int[] randomGap = gapTuples[Random.Range(0, gapTuples.Count)];
            int gapStart = randomGap[0];
            int gapWidth = randomGap[1];
            AddHatch(row, gapStart, gapWidth);
        }
    }

    private List<int[]> GetRowMapAsTuples(BitArray rowMap, bool inverse = false)
    {
        List<int[]> tuples = new List<int[]>();
        int segmentStart = 0;
        int segmentWidth = 0;

        for (int x = 0; x < rowMap.Length; x++)
        {
            bool include = (rowMap.Get(x) && !inverse) || (!rowMap.Get(x) && inverse);

            if (include)
            {
                segmentWidth += 1;
            }

            if (!include || x == MapWidth - 1)
            {
                if (segmentWidth > 0)
                {
                    int[] tuple = { segmentStart, segmentWidth };
                    tuples.Add(tuple);
                }
                segmentStart = x + 1;
                segmentWidth = 0;
            }
        }

        return tuples;
    }

}
