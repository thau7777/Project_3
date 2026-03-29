using MyRule.Event;
using System.Collections;
using UnityEngine;

public class FishingGameManager : MonoBehaviour
{
    public static FishingGameManager Instance;

    [Header("UI References")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    [SerializeField] private GameObject fishing;

    public int score;
    float time = 60f;
    int currentFisht;
    bool isGameEnded = false;

    void Awake()
    {
        Instance = this;
        
    }

    private void Update()
    {
        if (isGameEnded) return;

        UpdateUI();
        currentFisht = FishingSpawner.instance.fishCount;

        if (time <= 0 || currentFisht <= 0)
        {
            EndGame();
        }
    }

    public void UpdateUI()
    {
        scoreText.text = "Score: " + score;

        time -= Time.deltaTime;

        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"Time: {seconds:00}";
    }

    public void EndGame()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (time <= 0 && currentFisht > 0)
        {
            Debug.Log("Game Over! Final Score: " + score);
            EventBus<MiniGameResultEvent>.Raise(new MiniGameResultEvent(false));
            StartCoroutine(DelayAction(3f, () =>
            {
                fishing.SetActive(false);
            }));
        }
        else
        {
            Debug.Log("You Win! Final Score: " + score);
            EventBus<MiniGameResultEvent>.Raise(new MiniGameResultEvent(true));
            StartCoroutine(DelayAction(3f, () =>
            {
                fishing.SetActive(false);
            }));
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }
    public void Caught(float bonus)
    {
        time += bonus;
    }
    private IEnumerator DelayAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}