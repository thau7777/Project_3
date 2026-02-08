using System.Collections.Generic;
using UnityEngine;

namespace MyRule.UI
{
    public class TabView : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private List<Tab> tabNames;

        private int currentTabIndex = 0;

        private void OnEnable()
        {
            inputReader.uiActions.onNavigateTab += OnNavigateTab;
        }

        private void OnDisable()
        {
            inputReader.uiActions.onNavigateTab -= OnNavigateTab;
        }

        private void Start()
        {
            inputReader.SwitchActionMap(ActionMap.UI);
            UpdateTabSelection();
        }

        private void OnNavigateTab(int direction)
        {
            if (tabNames == null || tabNames.Count == 0) return;
            
            currentTabIndex += direction;

            if (currentTabIndex < 0)
                currentTabIndex = tabNames.Count - 1;
            else if (currentTabIndex >= tabNames.Count)
                currentTabIndex = 0;

            UpdateTabSelection();
        }

        private void UpdateTabSelection()
        {
            for (int i = 0; i < tabNames.Count; i++)
            {
                tabNames[i].SetSelected(i == currentTabIndex);
            }
        }
    }
}