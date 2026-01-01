using UnityEngine;
using System.Collections.Generic;

public class HexMazeGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public int width = 10;
    public int height = 10;
    public float hexRadius = 1f;
    public Transform startPoint;
    public Transform endPoint;

    private const float SQRT3 = 1.73205080757f;
    private Dictionary<Vector2Int, HexCell> grid = new();

    void Start()
    {
        GenerateHexGrid();
        CreateMainPath();
        CreateRandomBranches();
        SpawnMaze();
    }

    // === TẠO LƯỚI HEX ===
    void GenerateHexGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xOffset = (y % 2 == 0) ? 0f : SQRT3 / 2 * hexRadius;
                float worldX = x * SQRT3 * hexRadius + xOffset;
                float worldZ = y * 1.5f * hexRadius;
                Vector3 worldPos = new Vector3(worldX, 0, worldZ);
                Vector2Int gridPos = new Vector2Int(x, y);

                grid[gridPos] = new HexCell(gridPos, worldPos);
            }
        }

        // Gán láng giềng
        foreach (var cell in grid.Values)
        {
            foreach (Vector2Int offset in GetHexOffsets(cell.gridPos.y))
            {
                Vector2Int nPos = cell.gridPos + offset;
                if (grid.ContainsKey(nPos))
                    cell.neighbors.Add(grid[nPos]);
            }
        }
    }

    // === OFFSET LÁNG GIỀNG HEX ===
    List<Vector2Int> GetHexOffsets(int row)
    {
        if (row % 2 == 0)
            return new List<Vector2Int> {
                new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(-1, 1), new(-1, -1)
            };
        else
            return new List<Vector2Int> {
                new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1)
            };
    }

    // === TẠO ĐƯỜNG CHÍNH A → B ===
    void CreateMainPath()
    {
        Vector2Int start = new(0, 0);
        Vector2Int end = new(width - 1, height - 1);

        Stack<HexCell> stack = new();
        HashSet<HexCell> visited = new();

        HexCell startCell = grid[start];
        HexCell endCell = grid[end];

        stack.Push(startCell);

        while (stack.Count > 0)
        {
            HexCell current = stack.Pop();
            visited.Add(current);
            current.isPath = true;

            if (current == endCell)
                break;

            List<HexCell> shuffled = new List<HexCell>(current.neighbors);
            Shuffle(shuffled);

            foreach (var n in shuffled)
            {
                if (!visited.Contains(n))
                {
                    stack.Push(n);
                }
            }
        }
    }

    // === NHÁNH PHỤ NGẪU NHIÊN ===
    void CreateRandomBranches()
    {
        foreach (var cell in grid.Values)
        {
            if (cell.isPath && Random.value < 0.3f) // 30% sinh nhánh
            {
                foreach (var n in cell.neighbors)
                {
                    if (!n.isPath && Random.value < 0.5f)
                        n.isPath = true; // nhánh nhỏ
                }
            }
        }
    }

    // === SPAWN MÊ CUNG ===
    void SpawnMaze()
    {
        foreach (var cell in grid.Values)
        {
            GameObject hex = Instantiate(hexPrefab, cell.worldPos, Quaternion.identity, transform);
            hex.GetComponentInChildren<Renderer>().material.color = cell.isPath ? Color.yellow : Color.gray;
        }
    }

    // === TRỘN NGẪU NHIÊN ===
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}


public class HexCell
{
    public Vector2Int gridPos;      // vị trí trong lưới
    public Vector3 worldPos;        // vị trí trong world
    public bool isPath = false;     // có nằm trong đường chính không
    public List<HexCell> neighbors; // láng giềng

    public HexCell(Vector2Int gridPos, Vector3 worldPos)
    {
        this.gridPos = gridPos;
        this.worldPos = worldPos;
        neighbors = new List<HexCell>();
    }
}
