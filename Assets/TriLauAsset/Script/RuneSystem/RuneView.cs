using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class RuneView : MonoBehaviour, IRuneView
    {
        [SerializeField] private TextMeshProUGUI runeTxt;
        [SerializeField] private GameObject runeLockObj;
        [SerializeField] private TextMeshProUGUI runeLockTxt;

        private RunePresenter presenter;
        private int currentRune;

        private void Start()
        {
            presenter = new RunePresenter(this);
        }

        public async UniTask AdjustRune(int targetRune)
        {
            int start = currentRune;
            float time = 0f;
            float duration = 0.5f;

            while (time < duration)
            {
                time += Time.deltaTime;

                int value = Mathf.RoundToInt(Mathf.Lerp(start, targetRune, time / duration));

                runeTxt.text = value.ToString();

                await UniTask.Yield();
            }

            runeTxt.text = targetRune.ToString();
            currentRune = targetRune;
        }

        public void SetRuneLock(bool isLocked, string lockReason = "")
        {
            if (runeLockObj != null) runeLockObj.SetActive(isLocked);
            if (runeLockTxt != null) runeLockTxt.text = lockReason;
        }
    }
}