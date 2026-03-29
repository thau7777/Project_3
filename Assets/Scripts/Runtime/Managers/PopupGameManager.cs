using UnityEngine;
using TMPro;
using System;

public enum PopupGameType
{
    Fishing,
    Chatnoir,
}

public class PopupGameManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [Header("Mini Game Prefabs")]
    [SerializeField] private ChatNiorGameManager beatingPrefab;
    [SerializeField] private FishingGameManager fishingPrefab;
    // thêm các prefab khác nếu cần

    private PopupMiniGameUIBase currentGameUI;

    public void StartPopupGame(PopupGameType type)
    {
        EndCurrentPopup();

        inputReader.SwitchActionMap(ActionMap.PopUpGame);

        switch (type)
        {
            case PopupGameType.Chatnoir:
                //currentGameUI = Instantiate(beatingPrefab, transform);
                break;
            case PopupGameType.Fishing:
                //currentGameUI = Instantiate(fishingPrefab, transform);
                break;
        }

        currentGameUI.Init(inputReader, this);
        currentGameUI.Show();
    }

    public void EndCurrentPopup()
    {
        if (currentGameUI != null)
        {
            currentGameUI.Hide();
            Destroy(currentGameUI.gameObject);
            currentGameUI = null;

            // Trả lại input cũ cho player
            inputReader.SwitchActionMap(ActionMap.UI);
        }
    }
}


