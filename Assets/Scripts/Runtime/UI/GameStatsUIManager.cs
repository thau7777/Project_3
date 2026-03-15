using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameStatsUIManager : Singleton<GameStatsUIManager>
{
    [SerializeField]
    [TabGroup("Stats Box")]
    private GameObject _damageDealtBox;

    [SerializeField]
    [TabGroup("Stats Box")]
    private GameObject _damageReceivedBox;

    [SerializeField]
    [TabGroup("Stats Box")]
    private GameObject _parriedDamageBox;

    private TextMeshProUGUI _damageDealtText;
    private TextMeshProUGUI _damageReceivedText;
    private TextMeshProUGUI _parriedDamageText;


    public async UniTaskVoid Init(int damageDealt, int damageReceived, int parriedDamage)
    {
        Debug.LogWarning("ran");
        _damageDealtText = _damageDealtBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _damageReceivedText = _damageReceivedBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _parriedDamageText = _parriedDamageBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        NumberTransition(_damageDealtText, _damageDealtBox, damageDealt).Forget();
        await UniTask.Delay(200);
        NumberTransition(_damageReceivedText, _damageReceivedBox, damageReceived).Forget();
        await UniTask.Delay(200);
        NumberTransition(_parriedDamageText, _parriedDamageBox, parriedDamage).Forget();
    }

    private async UniTaskVoid NumberTransition(TextMeshProUGUI targetText, GameObject box, int to)
    {
        float elapsedTime = 0;
        float duration = 1.5f;
        float slideOffset = 50f;

        RectTransform rect = box.GetComponent<RectTransform>();
        Vector2 originalPos = rect.anchoredPosition;
        Vector2 startPos = originalPos - new Vector2(0, slideOffset);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            int currentNum = Mathf.RoundToInt(Mathf.Lerp(0, to, t));
            targetText.text = currentNum.ToString();

            rect.anchoredPosition = Vector2.Lerp(startPos, originalPos, t);
            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }

        targetText.text = to.ToString();
        rect.anchoredPosition = originalPos;
    }
}
