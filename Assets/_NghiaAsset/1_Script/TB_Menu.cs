using UnityEngine;
using UnityEngine.SceneManagement;

namespace Turnbase
{
    public class TB_Menu : MonoBehaviour
    {
        public static TB_Menu instance;

        public GameObject victoryMenu;

        public GameObject loseMenu;



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




        public void ShowVictoryMenu()
        {
            victoryMenu.SetActive(true);
        }

        public void ShowLoseMenu()
        {
            loseMenu.SetActive(true);
        }

        public void LoadSceneMain()
        {
            FlyweightFactory_TB.Instance.ClearAllPools();
            SceneManager.LoadScene("Map");
        }
    }

}