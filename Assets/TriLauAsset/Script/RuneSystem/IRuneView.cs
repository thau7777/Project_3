using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule.UI
{
    public interface IRuneView
    {
        void AdjustRune(int target);
        void SetRuneLock(bool isLocked, string lockReason = "");
    }
}