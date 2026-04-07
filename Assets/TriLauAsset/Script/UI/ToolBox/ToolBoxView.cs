using DG.Tweening;
using MyRule.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.ToolBox
{
    public class ToolBoxView : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Button escBtn;
        [SerializeField] private Button setWeatherBtn;

        [SerializeField] private TMP_InputField aSField;
        [SerializeField] private Button aSBtn;
        [SerializeField] private TMP_InputField pSField;
        [SerializeField] private Button pSBtn;

        [SerializeField] private Button addRune;


        private bool isShowing = false;

        private void OnEnable()
        {
            _inputReader.diceRollActions.onOpenToolBox += HandleToolBox;
            escBtn.onClick.AddListener(CloseToolBox);
            setWeatherBtn.onClick.AddListener(SetWeather);
            aSBtn.onClick.AddListener(AddAS);
            pSBtn.onClick.AddListener(AddPS);
            addRune.onClick.AddListener(AddRune);

        }

        private void OnDisable()
        {
            _inputReader.diceRollActions.onOpenToolBox -= HandleToolBox;
            escBtn.onClick.RemoveListener(CloseToolBox);
            setWeatherBtn.onClick.RemoveListener(SetWeather);
            aSBtn?.onClick.RemoveListener(AddAS);
            pSBtn?.onClick.RemoveListener(AddPS);
            addRune?.onClick.RemoveListener(AddRune);

        }

        private void HandleToolBox()
        {
            if (!isShowing)
            {
                OpenToolBox();
            }
            else
            {
                CloseToolBox();
            }
        }

        public void OpenToolBox()
        {
            if (isShowing) return;

            transform.DOLocalMoveX(-1370, 1f);
            isShowing = true;
        }

        public void CloseToolBox()
        {
            if (!isShowing) return;

            transform.DOLocalMoveX(-2500, 1f);
            isShowing = false;
        }

        private void SetWeather()
        {
            EventBus<ToolWeatherEvent>.Raise(new ToolWeatherEvent());
        }

        private void AddAS()
        {
            string id = aSField.text;

            SigilData sigilData = MatchManager.Instance.MatchData.SigilPool.GetActiveSigilById(id);

            if (sigilData == null) return;

            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

            SigilStorageManager.Instance.AddSigilToStorage(sigilSO);
        }

        private void AddPS()
        {
            string id = pSField.text;

            SigilData sigilData = MatchManager.Instance.MatchData.SigilPool.GetPassiveSigilById(id);

            if (sigilData == null) return;

            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

            SigilStorageManager.Instance.AddSigilToStorage(sigilSO);
        }

        private void AddRune()
        {
            EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(100));
        }
    }
}