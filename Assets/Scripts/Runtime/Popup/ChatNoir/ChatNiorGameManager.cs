using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatNiorGameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] int rows = 11;
    [SerializeField] int cols = 11;
    [SerializeField] string poolTag = "HexTile";
    [SerializeField] RectTransform gridParent;

    [Header("References")]
    [SerializeField] RectTransform catUI;
    [SerializeField] GameObject chatNoir;

    [Header("Hex Spacing")]
    [SerializeField] float horizontalSpacing = 1.05f;
    [SerializeField] float verticalSpacing = 1.05f;
    [SerializeField] float oddRowOffset = 0.5f;

    HexNode[,] grid;
    HexNode catNode;
    readonly List<HexNode> activeNodes = new();
    [Header("Game Setting")]
    [SerializeField] private bool isGameEnd = false;
    [SerializeField] private int moveCount;
    [SerializeField] private int score;

    
    private string winText = "Bạn đã bắt được mèo!";
    private string loseText = "Mèo đã trốn thoát!";
    private string scoreTextFormat = "Điểm: {0}";
    private string moveCountTextFormat = "Số bước: {0}";

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text moveCountText;
    [SerializeField] private TMP_Text resultText;

    #region Unity
    void Start()
    {

        SetUpStartGame();
    }
    private void Update()
    {
        UpdateUI();
    }
    #endregion

    #region UI Update
    public void UpdateUI()
    {
        
        scoreText.text = string.Format(scoreTextFormat, score);
        moveCountText.text = string.Format(moveCountTextFormat, moveCount);
        
    }
    
    
    public void SetUpStartGame()
    {
        StartNewGame();

        score = 2000;
        moveCount = 0;

        UpdateUI();
    }
    #endregion

    #region Game Flow
    public void StartNewGame()
    {

        ClearGrid();
        CreateGrid();
        SetupInitialScene();
        isGameEnd = false;
    }

    public void EndGame()
    {
        StartCoroutine(DelayedAction(2f, () =>
        {
            chatNoir.SetActive(false);
        }));
    }    
    void MoveCat()
    {
        HexNode next = FindSmartPath();

        if (next == null)
        {
            resultText.text = winText;
            isGameEnd = true;
            EndGame();
            return;
        }

        catNode = next;
        UpdateCatUI();

        if (IsAtEdge(catNode))
        {
            resultText.text = loseText;
            isGameEnd = true;
            EndGame();
        }
    }
    #endregion

    #region Grid
    void ClearGrid()
    {
        foreach (var n in activeNodes)
            n.gameObject.SetActive(false);

        activeNodes.Clear();
        grid = new HexNode[rows, cols];
    }

    void CreateGrid()
    {
        GameObject sample = PoolManager.Instance.SpawnFromPool(poolTag, gridParent);
        RectTransform sampleRT = sample.GetComponent<RectTransform>();
        float w = sampleRT.rect.width;
        float h = sampleRT.rect.height;
        sample.SetActive(false);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var go = PoolManager.Instance.SpawnFromPool(poolTag, gridParent);
                var node = go.GetComponent<HexNode>();
                var rt = go.GetComponent<RectTransform>();

                float xOffset = (r % 2 != 0) ? w * oddRowOffset : 0f;
                rt.anchoredPosition = new Vector2(
                    c * w * horizontalSpacing + xOffset,
                    -r * h * verticalSpacing
                );

                node.Init(r, c, OnNodeClicked);
                grid[r, c] = node;
                activeNodes.Add(node);
            }
        }
    }

    void SetupInitialScene()
    {
        catNode = grid[rows / 2, cols / 2];
        UpdateCatUI();

        int wallCount = Random.Range(4, 20);
        for (int i = 0; i < wallCount; i++)
        {
            int r = Random.Range(0, rows);
            int c = Random.Range(0, cols);
            if (grid[r, c] != catNode)
                grid[r, c].SetAsWall();
        }
    }
    #endregion
    

    #region Input
    void OnNodeClicked(HexNode node)
    {
        if (node.isBlocked || node == catNode||isGameEnd) return;

        node.SetAsWall();
        moveCount++;
        score = Mathf.Max(0, score - 100); // Mỗi bước trừ 100 điểm
        MoveCat();

    }
    #endregion

    #region Cat Pathfinding (Smart BFS)
    HexNode FindSmartPath()
    {
        Queue<HexNode> queue = new();
        Dictionary<HexNode, HexNode> parent = new();
        Dictionary<HexNode, int> dist = new();
        List<HexNode> exits = new();

        queue.Enqueue(catNode);
        parent[catNode] = null;
        dist[catNode] = 0;

        while (queue.Count > 0)
        {
            HexNode cur = queue.Dequeue();

            if (IsAtEdge(cur))
                exits.Add(cur);

            foreach (var n in GetNeighbors(cur))
            {
                if (n.isBlocked || parent.ContainsKey(n)) continue;

                parent[n] = cur;
                dist[n] = dist[cur] + 1;
                queue.Enqueue(n);
            }
        }

        if (exits.Count == 0) return null;

        HexNode bestExit = exits
            .OrderBy(e => dist[e])
            .ThenBy(HeuristicToEdge)
            .First();

        HexNode step = bestExit;
        while (parent[step] != catNode)
            step = parent[step];

        return step;
    }

    int HeuristicToEdge(HexNode n)
    {
        return Mathf.Min(
            n.row,
            rows - 1 - n.row,
            n.col,
            cols - 1 - n.col
        );
    }
    #endregion

    #region Helpers
    void UpdateCatUI()
    {
        catUI.anchoredPosition =
            catNode.GetComponent<RectTransform>().anchoredPosition;
    }

    bool IsAtEdge(HexNode n)
    {
        return n.row == 0 || n.row == rows - 1 ||
               n.col == 0 || n.col == cols - 1;
    }

    static readonly int[][] EVEN =
    {
        new[]{0,1}, new[]{0,-1},
        new[]{1,0}, new[]{-1,0},
        new[]{1,-1}, new[]{-1,-1}
    };

    static readonly int[][] ODD =
    {
        new[]{0,1}, new[]{0,-1},
        new[]{1,0}, new[]{-1,0},
        new[]{1,1}, new[]{-1,1}
    };

    List<HexNode> GetNeighbors(HexNode n)
    {
        var result = new List<HexNode>();
        var offsets = (n.row % 2 == 0) ? EVEN : ODD;

        foreach (var o in offsets)
        {
            int r = n.row + o[0];
            int c = n.col + o[1];

            if (r >= 0 && r < rows && c >= 0 && c < cols)
                result.Add(grid[r, c]);
        }
        return result;
    }
    private IEnumerator DelayedAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    #endregion
}