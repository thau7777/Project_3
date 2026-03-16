using MyRule.Event;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class MatchResultView : MonoBehaviour
    {
        [SerializeField] private GameObject matchResultContents;
        [SerializeField] private GameObject winTitle;
        [SerializeField] private GameObject loseTitle;
        [SerializeField] private Button submitBtn;

        private EventBinding<UpdateMatchResultEvent> matchResulteventBinding;

        private void OnEnable()
        {
            matchResulteventBinding = new EventBinding<UpdateMatchResultEvent>(CheckMatchResult);
            EventBus<UpdateMatchResultEvent>.Register(matchResulteventBinding);
        }

        private void OnDisable()
        {
            EventBus<UpdateMatchResultEvent>.Deregister(matchResulteventBinding);
        }

        private void Start()
        {
            Hide();
            submitBtn.onClick.AddListener(OnSubmit);
        }

        private void CheckMatchResult(UpdateMatchResultEvent evt)
        {
            if (evt.eMatchResult == EMatchResult.Win)
            {
                Show(winTitle);
            }
            else if (evt.eMatchResult == EMatchResult.Lose)
            {
                Show(loseTitle);
            }
        }

        private void Show(GameObject title)
        {
            matchResultContents.SetActive(true);
            title.SetActive(true);

            submitBtn.Select();
        }

        private void Hide()
        {
            matchResultContents?.SetActive(false);
            winTitle?.SetActive(false);
            loseTitle?.SetActive(false);
        }

        private async void OnSubmit()
        {
            MatchData matchData = MatchManager.Instance.MatchData;
            HistoryManager.Instance.AddMatchToHistory(matchData);
            MatchManager.Instance.FinishMatch();

            await Loader.LoadSceneWithLoading(Loader.EScene.SpaceStationScene);
        }
    }
}