using UnityEngine;
using TMPro;
using System;

public enum PopupGameType
{
    Fishing,
    Racing,
    Reflecting,
    Quizzing,
    Beating // game rhythm
}

public class PopupGameManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [Header("Mini Game Prefabs")]
    //[SerializeField] private UpdateArrownUI beatingPrefab;
    //[SerializeField] private FishingGameUI fishingPrefab;
    // thêm các prefab khác nếu cần

    private PopupMiniGameUIBase currentGameUI;

    public void StartPopupGame(PopupGameType type)
    {
        EndCurrentPopup();

        // Chuyển sang input UI game
        inputReader.SwitchActionMap(ActionMap.PopUpGame);

        switch (type)
        {
            case PopupGameType.Beating:
                //currentGameUI = Instantiate(beatingPrefab, transform);
                break;
            case PopupGameType.Fishing:
                //currentGameUI = Instantiate(fishingPrefab, transform);
                break;
                // thêm các loại khác ở đây
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
            inputReader.SwitchActionMap(ActionMap.PlayerTopDown);
        }
    }
}


