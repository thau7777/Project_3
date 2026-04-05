using UnityEngine;

namespace MyRule
{
    public class MiniGameSetter : MonoBehaviour
    {
        [SerializeField] private GameObject fishGame;
        [SerializeField] private GameObject beastGame;

        private void Start()
        {
            fishGame.SetActive(false);
            beastGame.SetActive(false);
            SetGame();
        }

        private void SetGame()
        {
            string name = MiniGameManager.Instance.MGName;

            switch (name)
            {
                case "Fish":
                    fishGame.gameObject.SetActive(true);
                    break;
                case "Beast":
                    beastGame.gameObject.SetActive(true);
                    break;
            }
        }
    }
}