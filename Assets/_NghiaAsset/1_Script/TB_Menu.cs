using MyRule;
using System.Threading.Tasks;
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

            CombatManager.Instance.SetCombatResultWin();

            isVicrory = true;
        }

        public void ShowLoseMenu()
        {
            loseMenu.SetActive(true);

            CombatManager.Instance.SetCombatResultLose();

            isVicrory = false;
        }

        public async void LoadSceneMain(bool result)
        {

            Debug.Log("Adjust the number of runes here.");

            EventBus<TBVictoryEvent>.Raise(new TBVictoryEvent(result));
            
            await Loader.LoadSceneDirect(Loader.EScene.MazeScene);

            //FlyweightFactory_TB.Instance.ClearAllPools();
            //SceneManager.LoadScene("Map");
        }
    }

}