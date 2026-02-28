using UnityEngine;

public class FishingGameManager : MonoBehaviour
{
    public static FishingGameManager Instance;


    [Header("UI References")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    private string timerFormat = "mm:ss";
    public int score;
    float time = 60f;


    void Awake()
    {
        Instance = this;
        
    }
    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        time -= Time.deltaTime;
        timerText.text = "Time: " + ((int)time).ToString();
    }
    public void AddScore(int value)
    {
        score += value;
        Debug.Log("Score: " + score);
    }
}