using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Turnbase
{
    public class CombatantButton : MonoBehaviour
    {
        [Header("References")]
        public Button button;
        public Image avatarImage;

        private Character character;
        private CharacterStatUI statUI;



        public void Setup(Character combatant, CharacterStatUI statUIRef)
        {
            character = combatant;
            statUI = statUIRef;
            avatarImage.sprite = character.stats.Avatar;



            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);



        }
        private void OnButtonClick()
        {
            if (statUI != null)
            {
                statUI.ShowStats(character);
            }
        }






    }  
}
