using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;


namespace Turnbase
{
    public class TurnOrderUI : MonoBehaviour
    {
        public GameObject turnOrderIconPrefab;

        public Transform turnOrderContainer;

        public CharacterStatUI statDisplayPanel;

        public BattleManager battleManager;

        private List<TurnOrderIcon> turnOrderIcons = new List<TurnOrderIcon>();

        public void UpdateTurnQueue(List<Character> characters)
        {
            if (statDisplayPanel == null || battleManager == null)
            {
                Debug.LogError("TurnOrderUI: Thiếu tham chiếu Stat Display Panel hoặc Battle Manager! Vui lòng gán trong Inspector.");
                return;
            }

            List<Character> sortedCharacters = characters.Where(c => c.isAlive).OrderByDescending(c => c.actionGauge).ToList();

            foreach (var icon in turnOrderIcons)
            {
                Destroy(icon.gameObject);
            }
            turnOrderIcons.Clear();

            foreach (Character character in sortedCharacters)
            {
                GameObject iconObj = Instantiate(turnOrderIconPrefab, turnOrderContainer);

                TurnOrderIcon iconComponent = iconObj.GetComponent<TurnOrderIcon>();

                if (iconComponent != null)
                {
                    turnOrderIcons.Add(iconComponent);

                    iconComponent.Setup(character, statDisplayPanel, battleManager);
                }
            }
        }

        public void HighlightActiveCharacter(Character activeCharacter)
        {
            foreach (var icon in turnOrderIcons)
            {
                icon.transform.localScale = Vector3.one;
            }

            foreach (var icon in turnOrderIcons)
            {
                if (icon.characterOwner == activeCharacter)
                {
                    icon.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    break;
                }
            }
        }

        public void UpdateActionGaugeUI(List<Character> characters)
        {
            UpdateTurnQueue(characters);
        }
    }
}