using System.Collections.Generic;
using UnityEngine;

public class CatGameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 11;
    public int cols = 11;
    public string poolTag = "HexTile";
    public RectTransform gridParent;

    [Header("References")]
    public RectTransform catUI; // Object hình con mèo
    public GameObject parent; // Đối tượng cha để tổ chức trong hierarchy

    private HexNode[,] grid;
    private HexNode catNode;
    private List<HexNode> activeNodes = new List<HexNode>();

    // ---------------------------------------------------------
    // CÔNG THỨC VÀNG ĐỂ KHÔNG BỊ RỐI BIẾN
    // ---------------------------------------------------------


    private void Start()
    {
        StartNewGame();
    }
    private void OnDisable()
    {
        
    }
    public void StartNewGame()
    {
        
        
        // 1. Dọn dẹp lưới cũ (trả về pool)
        foreach (var node in activeNodes) node.gameObject.SetActive(false);
        activeNodes.Clear();

        grid = new HexNode[rows, cols];

        // 2. Lấy thử 1 mẫu để đo kích thước (Width/Height)
        GameObject sample = PoolManager.Instance.SpawnFromPool(poolTag, gridParent);
        RectTransform sampleRT = sample.GetComponent<RectTransform>();

        float w = sampleRT.rect.width;   // Chiều rộng thật
        float h = sampleRT.rect.height;  // Chiều cao thật

        // Trả mẫu về ngay lập tức để vòng lặp bên dưới dùng
        sample.SetActive(false);

        // 3. Vòng lặp tạo lưới
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject go = PoolManager.Instance.SpawnFromPool(poolTag, gridParent);
                HexNode node = go.GetComponent<HexNode>();
                RectTransform rt = go.GetComponent<RectTransform>();

                // A. Tính toán vị trí X (Lệch 0.5w ở hàng lẻ)
                float xOffset = (r % 2 != 0) ? (w / 2f) : 0;
                float xPos = (c * w) + xOffset;

                // B. Tính toán vị trí Y (Khớp 0.75h để vào tổ ong)
                float yPos = r * (h * 0.75f);

                // C. Áp dụng vào UI (Y âm để đi xuống dưới)
                rt.anchoredPosition = new Vector2(xPos, -yPos);
                rt.localScale = Vector3.one; // Cố định scale = 1

                node.Init(r, c, OnNodeClicked);
                grid[r, c] = node;
                activeNodes.Add(node);
            }
        }

        SetupInitialScene();
    }

    private void SetupInitialScene()
    {
        // Đặt mèo vào giữa
        catNode = grid[rows / 2, cols / 2];
        UpdateCatUI();

        // Random một vài tường (ví dụ 10 ô)
        for (int i = 0; i < 10; i++)
        {
            int r = Random.Range(0, rows);
            int c = Random.Range(0, cols);
            if (grid[r, c] != catNode) grid[r, c].SetAsWall();
        }
    }

    private void OnNodeClicked(HexNode clickedNode)
    {
        if (clickedNode.isBlocked || clickedNode == catNode) return;

        clickedNode.SetAsWall();
        // Sau khi người chơi đặt tường, cho mèo di chuyển (AI BFS đã viết ở trên)
        MoveCat();
    }
    void MoveCat()
    {
        HexNode nextStep = FindBFSPath();

        if (nextStep == null)
        {
            Debug.Log("Chúc mừng! Bạn đã bắt được mèo!");
            return;
        }

        catNode = nextStep;
        UpdateCatUI();

        // Kiểm tra mèo thua (ra tới biên)
        if (catNode.row == 0 || catNode.row == rows - 1 || catNode.col == 0 || catNode.col == cols - 1)
        {
            Debug.Log("Mèo đã trốn thoát! Game Over!");
        }
    }

    private void UpdateCatUI()
    {
        catUI.anchoredPosition = catNode.GetComponent<RectTransform>().anchoredPosition;
    }
    // Thuật toán tìm đường BFS
    HexNode FindBFSPath()
    {
        Queue<HexNode> queue = new Queue<HexNode>();
        Dictionary<HexNode, HexNode> parentMap = new Dictionary<HexNode, HexNode>();

        queue.Enqueue(catNode);
        parentMap[catNode] = null;

        HexNode escapeNode = null;

        while (queue.Count > 0)
        {
            HexNode current = queue.Dequeue();

            if (IsAtEdge(current))
            {
                escapeNode = current;
                break;
            }

            foreach (HexNode neighbor in GetNeighbors(current))
            {
                if (!neighbor.isBlocked && !parentMap.ContainsKey(neighbor))
                {
                    parentMap[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (escapeNode != null)
        {
            // Truy vết ngược lại để tìm bước đi tiếp theo ngay sát con mèo
            HexNode step = escapeNode;
            while (parentMap[step] != catNode && parentMap[step] != null)
            {
                step = parentMap[step];
            }
            return step;
        }

        return null;
    }

    bool IsAtEdge(HexNode node) => node.row == 0 || node.row == rows - 1 || node.col == 0 || node.col == cols - 1;

    List<HexNode> GetNeighbors(HexNode node)
    {
        List<HexNode> neighbors = new List<HexNode>();
        int[][] offsets = (node.row % 2 == 0) ?
            new int[][] { new[] { 0, 1 }, new[] { 0, -1 }, new[] { 1, 0 }, new[] { -1, 0 }, new[] { 1, -1 }, new[] { -1, -1 } } :
            new int[][] { new[] { 0, 1 }, new[] { 0, -1 }, new[] { 1, 0 }, new[] { -1, 0 }, new[] { 1, 1 }, new[] { -1, 1 } };

        foreach (var off in offsets)
        {
            int r = node.row + off[0];
            int c = node.col + off[1];
            if (r >= 0 && r < rows && c >= 0 && c < cols) neighbors.Add(grid[r, c]);
        }
        return neighbors;
    }
}
