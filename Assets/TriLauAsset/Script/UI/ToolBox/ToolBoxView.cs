using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.ToolBox
{
    public class ToolBoxView : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Button escBtn;

        private bool isShowing = false;

        private void OnEnable()
        {
            _inputReader.diceRollActions.onOpenToolBox += HandleToolBox;
            escBtn.onClick.AddListener(CloseToolBox);
        }

        private void OnDisable()
        {
            _inputReader.diceRollActions.onOpenToolBox -= HandleToolBox;
            escBtn.onClick.RemoveListener(CloseToolBox);
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
    }
}