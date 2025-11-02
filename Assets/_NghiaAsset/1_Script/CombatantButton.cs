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

        public CameraViewManager cameraViewManager;

        private Character character;
        private CharacterStatUI statUI;

        void Start()
        {
            if (cameraViewManager == null)
            {
                cameraViewManager = FindFirstObjectByType<CameraViewManager>();
            }
        }

        public void Setup(Character combatant, CharacterStatUI statUIRef)
        {
            character = combatant;
            statUI = statUIRef;
            avatarImage.sprite = character.info.Avatar;



            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);



        }
        private void OnButtonClick()
        {
            if (statUI != null)
            {
                statUI.ShowStats(character);
            }
            
            if (cameraViewManager != null)
            {
                cameraViewManager.SetCameraView(character);
            }

        }






    }  
}
