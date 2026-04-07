using Cysharp.Threading.Tasks;
using System.Threading;
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

        private CancellationTokenSource cts;

        private void Start()
        {
            presenter = new RunePresenter(this);
        }

        public void AdjustRune(int targetRune)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            int start = currentRune;

            Transition.TransitionValue(
                setter: value => runeTxt.text = ((int)value).ToString(),
                from: currentRune,
                to: targetRune,
                duration: 0.5f,
                token: cts.Token).Forget();

            currentRune = targetRune;
        }

        public void SetRuneLock(bool isLocked, string lockReason = "")
        {
            if (runeLockObj != null) runeLockObj.SetActive(isLocked);
            if (runeLockTxt != null) runeLockTxt.text = lockReason;
        }
    }
}