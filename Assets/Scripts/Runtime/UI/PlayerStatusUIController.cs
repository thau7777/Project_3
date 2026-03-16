using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatusUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _hpGo;
    [SerializeField]
    private Slider _manaSlider;
    [SerializeField]
    private GameObject _manaGo;

    [SerializeField]
    private TextMeshProUGUI _hpText;
    [SerializeField]
    private TextMeshProUGUI _manaText;

    private Material HPMat;
    private EventBinding<TopdownSkillOnUseEvent> _skillOnUseEvent;

    private void Awake()
    {
        HPMat = _hpGo.GetComponent<Image>().material;
    }
    private void OnEnable()
    {
        _skillOnUseEvent = new(OnNotEnoughMana);
        EventBus<TopdownSkillOnUseEvent>.Register(_skillOnUseEvent);
    }
    private void OnDisable()
    {
        EventBus<TopdownSkillOnUseEvent>.Deregister(_skillOnUseEvent);
    }
    public void InitializePlayerStatus(float currentHP, float maxHP, float currentMana, float maxMana)
    {
        UpdateHealth(currentHP, maxHP);
        UpdateMana(currentMana, maxMana);
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if(HPMat != null)
        {
            float hpRatio = currentHP / maxHP;
            HPMat.SetFloat("_FillLevel", hpRatio);
        }
        if(_hpText != null)
        {
            _hpText.text = Mathf.RoundToInt(currentHP).ToString() + " / " + Mathf.RoundToInt(maxHP).ToString();
        }
    }
    public void UpdateMana(float currentMana, float maxMana)
    {
        if(_manaSlider != null)
        {
            _manaSlider.value = currentMana / maxMana;
        }
        if(_manaText != null)
        {
            _manaText.text = Mathf.RoundToInt(currentMana).ToString() + " / " + Mathf.RoundToInt(maxMana).ToString();
        }
    }
    private void OnNotEnoughMana(TopdownSkillOnUseEvent topdownSkillOnUseEvent)
    {
        if (topdownSkillOnUseEvent.skillOnUseState != SkillOnUseState.NotEnoughMana) return;
        TriggerNotEnoughManaEffect().Forget();
    }

    private async UniTaskVoid TriggerNotEnoughManaEffect()
    {
        float duration = 0.4f;
        float magnitude = 2f;
        float elapsed = 0f;

        RectTransform rectTransform = _manaGo.GetComponent<RectTransform>();
        Vector2 originalPosition = rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPosition.y + Random.Range(-1f, 1f) * magnitude;

            rectTransform.anchoredPosition = new Vector2(x, y);

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }

        rectTransform.anchoredPosition = originalPosition;
    }
}
