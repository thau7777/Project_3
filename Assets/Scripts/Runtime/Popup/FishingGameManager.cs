using Ami.BroAudio;
using MyRule;
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
    [SerializeField] private InputReader inputReader;
    [TabGroup("BroAudio")]
    [SerializeField] private SoundID bgm;


    public int score;
    [SerializeField]private float time = 60f;
    int currentFisht;
    bool isGameEnded = false;
    //public bool useTimer = true;
    void Awake()
    {
        Instance = this;

    }
    private void OnEnable()
    {
        bgm.Play();

    }
    private void OnDisable()
    {
        BroAudio.Stop(bgm);
    }

    private void Update()
    {
        if (isGameEnded) return;

        UpdateUI();
        currentFisht = FishingSpawner.instance.fishCount;
        //if (!useTimer) return;
        if (time <= 0 || currentFisht <= 0)
        {
            EndGame();
        }
    }

    public void UpdateUI()
    {
        scoreText.text = "Score: " + score;

        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 0;
            return;
        }
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"Time: {seconds:00}";
    }

    public void EndGame()
    {

        if (isGameEnded) return;
        isGameEnded = true;

        inputReader.SwitchActionMap(ActionMap.DiceRoll);
        
        if (time <= 0 && currentFisht > 0)
        {
            Debug.Log("Game Over! Final Score: " + score);
            EventBus<MiniGameResultEvent>.Raise(new MiniGameResultEvent(false));
            EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent((int)(score * 0.1)));
            StartCoroutine(DelayAction(0.5f, () =>
            {
                fishing.SetActive(false);
            }));
        }
        else
        {
            Debug.Log("You Win! Final Score: " + score);
            EventBus<MiniGameResultEvent>.Raise(new MiniGameResultEvent(true));
            EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent((int)(score * 0.1)));
            StartCoroutine(DelayAction(0.5f, () =>
            {
                fishing.SetActive(false);
            }));
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }
    
    private IEnumerator DelayAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}