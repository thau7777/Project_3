using UnityEngine;

namespace MyRule.UI
{
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] private TabView _tabView;

        private void Start()
        {
            _tabView.ResetTab();
        }
    }
}