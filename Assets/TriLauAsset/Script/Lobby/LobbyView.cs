using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class LobbyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldTxt;
        [SerializeField] private TextMeshProUGUI crystalTxt;
        [SerializeField] private float transitionDuration = 0.4f;

        private LobbyPresenter lobbyPresenter;

        private void OnEnable()
        {
            lobbyPresenter = new LobbyPresenter(goldTxt, crystalTxt, transitionDuration);
        }

        private void OnDisable()
        {
            lobbyPresenter.Clearup();
        }
    }
}