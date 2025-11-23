using Turnbase;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    // Kéo thả ElementColorMap Asset vào đây
    public ElementColorMap elementColorMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại giữa các Scene (Tùy chọn)
        }
    }
}