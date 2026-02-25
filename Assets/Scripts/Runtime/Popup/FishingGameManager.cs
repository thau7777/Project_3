using UnityEngine;

public class FishingGameManager : MonoBehaviour
{
    public static FishingGameManager Instance;

    public int score;

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int value)
    {
        score += value;
        Debug.Log("Score: " + score);
    }
}