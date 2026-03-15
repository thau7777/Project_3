using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class MatchResultView : MonoBehaviour
    {
        [SerializeField] private GameObject matchResultContents;
        [SerializeField] private GameObject winTitle;
        [SerializeField] private GameObject loseTitle;

        private async void Start()
        {
            Hide();

            await UniTask.WaitUntil(() => MatchManager.Instance.MatchData != null);

            CheckMatchResult();  
        }

        private void CheckMatchResult()
        {
            MatchData matchData = MatchManager.Instance.MatchData;

            if (matchData.Result == EMatchResult.Win)
            {
                Show(winTitle);
            }
            else if (matchData.Result == EMatchResult.Lose)
            {
                Show(loseTitle);
            }
        }

        private void Show(GameObject title)
        {
            matchResultContents.SetActive(true);
            title.SetActive(true);
        }

        private void Hide()
        {
            matchResultContents?.SetActive(false);
            winTitle?.SetActive(false);
            loseTitle?.SetActive(false);
        }
    }
}