using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ParriedVolumeController : MonoBehaviour
{
    public static ParriedVolumeController Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Volume cần điều khiển (Nên là cái Parried Volume)")]
    [SerializeField] private Volume parriedVolume;
    
    [Tooltip("Thời gian giảm Weight từ 1 về 0")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CancellationTokenSource cts;

    private void Awake()
    {
        // Setup Singleton để dễ dàng gọi từ bất kỳ đâu (như ParryCommand)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Nếu chưa gán volume, tự động lấy Volume component ở object hiện tại
        if (parriedVolume == null)
        {
            parriedVolume = GetComponent<Volume>();
        }

        // Đảm bảo lúc bắt đầu thì weight là 0
        if (parriedVolume != null)
        {
            parriedVolume.weight = 0f;
        }
    }

    private void Start()
    {
        cts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        cts?.Cancel();
        cts?.Dispose();
    }

    /// <summary>
    /// Gọi hàm này để kích hoạt Volume lên 1 sau đó mờ dần về 0
    /// </summary>
    public void TriggerParriedEffect()
    {
        if (parriedVolume == null) return;
        
        // Hủy quá trình fade trước đó nếu có để tránh bị đụng độ
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        FadeVolumeTask(cts.Token).Forget();
    }

    private async UniTask FadeVolumeTask(CancellationToken token)
    {
        try 
        {
            // Bật weight lên 1 ngay lập tức khi Perfect Parry
            parriedVolume.weight = 1f;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                
                // Giảm dần từ 1 về 0
                parriedVolume.weight = Mathf.Lerp(1f, 0f, t);
                
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            // Đảm bảo cuối cùng về đúng 0
            parriedVolume.weight = 0f;
        }
        catch (System.OperationCanceledException)
        {
            // Bị cancel (ví dụ khi gọi lại TriggerParriedEffect liên tục hoặc object bị destroy)
        }
    }
}
