using UnityEngine;
using TMPro;

public class BeatingGameUI : PopupMiniGameUIBase
{
    [SerializeField] private TextMeshProUGUI targetKeyText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private string currentKey;
    private int score;

    public override void Show()
    {
        base.Show();

        inputReader.popUpGame.onLeftClick += OnLeftClick;
        inputReader.popUpGame.onRightClick += OnRightClick;

        score = 0;
        scoreText.text = "Điểm: 0";
        SpawnNewKey();
    }

    private void OnDestroy()
    {
        inputReader.popUpGame.onLeftClick -= OnLeftClick;
        inputReader.popUpGame.onRightClick -= OnRightClick;
    }

    private void SpawnNewKey()
    {
        string[] keys = { "LeftClick", "RightClick" };
        currentKey = keys[Random.Range(0, keys.Length)];
        targetKeyText.text = $"Nhấn: {currentKey}";
    }

    private void OnLeftClick() => Check("LeftClick");
    private void OnRightClick() => Check("RightClick");

    private void Check(string pressed)
    {
        if (pressed == currentKey)
        {
            score++;
            scoreText.text = $"Điểm: {score}";
        }
        else
        {
            Debug.Log("Sai nút!");
        }

        SpawnNewKey();
    }

    // Gọi khi người chơi kết thúc hoặc thoát mini-game
    public void OnExitButton()
    {
        manager.EndCurrentPopup();
    }
}
