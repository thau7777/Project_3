using UnityEngine;

namespace MyRule
{
    public interface IHUDView
    {
        void ShowHUD();
        void HideHUD();

        void ShowStorage();
        void HideStorage();

        void ShowPassiveSigilStorage();
        void HidePassiveSigilStorage();
    }
}