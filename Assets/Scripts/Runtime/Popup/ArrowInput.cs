using UnityEngine;
using System.Collections.Generic;
using Ami.BroAudio;

public class ArrowInput : MonoBehaviour
{
    public ArrowManager arrowManager;

    private FishingUI fishingUI;
    private bool canInput = false;

    private Dictionary<KeyCode, ArrowType> keyMap;
    [TabGroup("BroAudio")]
    [SerializeField] private SoundID  arrow;

    void Awake()
    {
        keyMap = new Dictionary<KeyCode, ArrowType>
        {
            { KeyCode.LeftArrow, ArrowType.Left },
            { KeyCode.UpArrow, ArrowType.Up },
            { KeyCode.DownArrow, ArrowType.Down },
            { KeyCode.RightArrow, ArrowType.Right }
        };
    }

    void Update()
    {
        if (!canInput) return;

        foreach (var key in keyMap)
        {
            if (Input.GetKeyDown(key.Key))
            {
                CheckInput(key.Value);
                break;
            }
        }
    }

    public void EnableInput(bool value)
    {
        canInput = value;
    }

    public void SetFishingUI(FishingUI ui)
    {
        fishingUI = ui;
    }

    void CheckInput(ArrowType input)
    {
        bool correct = input == arrowManager.currentArrow;

        fishingUI.ModifyTimer(correct);
        arrow.Play();

        if (correct)
            arrowManager.NextArrow();

        Debug.Log(correct ? "ĐÚNG!" : "SAI!");
    }
}