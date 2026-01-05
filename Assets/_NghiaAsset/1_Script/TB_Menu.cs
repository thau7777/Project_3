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
        }

        public void ShowLoseMenu()
        {
            loseMenu.SetActive(true);
        }

        public void LoadSceneMain(bool result)
        {
            EventBus<TBVictoryEvent>.Raise(new TBVictoryEvent(result));
            SceneManager.LoadScene("BoardScene");

            //FlyweightFactory_TB.Instance.ClearAllPools();
            //SceneManager.LoadScene("Map");
        }
    }

}