using UnityEngine;
using UnityEngine.UI;

public class FishingUI : MonoBehaviour
{
    [Header("UI")]
    public Image fishingBar;
    public GameObject arrowUI;

    [Header("Settings")]
    public float fishingDuration = 5f;

    private float timer;
    private bool isFishing = false;

    [Header("References")]
    public ArrowManager arrowManager;
    public ArrowInput arrowInput;

    void Start()
    {
        fishingBar.fillAmount = 0f;
        fishingBar.gameObject.SetActive(false);
        arrowUI.SetActive(false);
    }

    void Update()
    {
        if (!isFishing && Input.GetKeyDown(KeyCode.Space))
        {
            StartFishing();
        }

        if (isFishing)
        {
            timer += Time.deltaTime*0.05f; // thanh tự chạy

            timer = Mathf.Clamp(timer, 0f, fishingDuration);
            fishingBar.fillAmount = timer / fishingDuration;

            if (timer >= fishingDuration)
            {
                EndFishing(true);
            }
            else if (timer <= 0f)
            {
                EndFishing(false);
            }
        }
    }

    void StartFishing()
    {
        isFishing = true;
        timer = fishingDuration *0.1f;

        fishingBar.gameObject.SetActive(true);
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

        Debug.Log(success ? "CÂU THÀNH CÔNG!" : "CÁ CHẠY MẤT!");
    }

    // 👉 Được ArrowInput gọi khi player bấm
    public void ModifyTimer(bool correct)
    {
        float value = Time.deltaTime + 0.5f;

        if (correct)
            timer += value;
        else
            timer -= value;
    }
}