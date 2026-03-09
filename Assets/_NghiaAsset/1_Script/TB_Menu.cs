using MyRule;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Turnbase
{
    public class TB_Menu : MonoBehaviour
    {
        public static TB_Menu instance;

        public GameObject victoryMenu;

        public GameObject loseMenu;

        public Button victoryCloseBtn;

        public Button loseCloseBtn;

        private bool isVicrory = false;

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            victoryCloseBtn.onClick.AddListener(() => LoadSceneMain(true));
            loseCloseBtn.onClick.AddListener(() => LoadSceneMain(false));
        }


        public void ShowVictoryMenu()
        {
            victoryMenu.SetActive(true);

            isVicrory = true;
        }

        public void ShowLoseMenu()
        {
            loseMenu.SetActive(true);
            isVicrory = false;
        }

        public void LoadSceneMain(bool result)
        {
            Debug.Log("Adjust the number of runes here.");
            int runAmount = 100;
            MazeGameplayRewardManager.Instance.CreateNewReward(runAmount);

            EventBus<TBVictoryEvent>.Raise(new TBVictoryEvent(result));
            
            SceneManager.LoadScene("MazeScene");

            //FlyweightFactory_TB.Instance.ClearAllPools();
            //SceneManager.LoadScene("Map");
        }
    }

}