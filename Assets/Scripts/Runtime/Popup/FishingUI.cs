using UnityEngine;
using UnityEngine.UI;

public class FishingUI : MonoBehaviour
{
    public static FishingUI instance;

    [Header("UI")]
    public Image fishingBar;
    public GameObject arrowUI;
    public GameObject fishBar;

    [Header("Settings")]
    public float duration = 5f;
    private float timer;
    private bool isFishing = false;
    public float fishTime = 1f;

    [Header("References")]
    public ArrowManager arrowManager;
    public ArrowInput arrowInput;

    private HookController currentHook;

    private void Awake()
    {
        instance = this;
        fishBar.SetActive(false);
        arrowUI.SetActive(false);
    }

    void Update()
    {
        if (!isFishing) return;

        timer += Time.deltaTime* fishTime;
        fishingBar.fillAmount = timer / duration;

        if (timer >= duration)
            EndFishing(true);
        else if (timer <= 0f)
            EndFishing(false);
    }

    public void StartFishing(HookController hook)
    {
        currentHook = hook;

        isFishing = true;
        timer = duration *0.1f;

        fishBar.SetActive(true);
        arrowUI.SetActive(true);

        arrowManager.NextArrow();
        arrowInput.EnableInput(true);
        arrowInput.SetFishingUI(this);
    }

    void EndFishing(bool success)
    {
        isFishing = false;

        arrowInput.EnableInput(false);
        arrowUI.SetActive(false);
        fishBar.SetActive(false);

        currentHook.PullUp(success);
        Debug.Log(success ? "CÂU THÀNH CÔNG!" : "CÁ CHẠY MẤT!");
        
    }

    public void ModifyTimer(bool correct)
    {
        float value = Time.deltaTime + 0.5f;

        timer += correct ? value : -value;
    }
}