using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule.UI
{
    public interface IRuneView
    {
        UniTask AdjustRune(int target);
        void SetRuneLock(bool isLocked, string lockReason = "");
    }
}