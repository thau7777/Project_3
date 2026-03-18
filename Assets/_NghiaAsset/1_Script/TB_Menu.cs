using System.Collections;
using Cysharp.Threading.Tasks;
using MyRule;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

        public Volume globalVolume;
        private Vignette vignette;

        public BattleSpawner spawner;

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
            if (globalVolume.profile.TryGet<Vignette>(out var v))
            {
                vignette = v;
            }

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

            loseMenu.SetActive(false);
            victoryMenu.SetActive(false);

            vignette.intensity.value = 0f;
            vignette.smoothness.value = 0f;

            CameraAction.instance.TargetAllEnemies();

            spawner.playerOffsetFromSlot = new Vector3(10.8f, 0, -6);
            Vector3 targetPos = spawner.playerOffsetFromSlot;
            spawner.WarpDriveBackk(targetPos);

            await UniTask.Delay(5000);
            
            await Loader.LoadSceneDirect(Loader.EScene.MazeScene);

            //FlyweightFactory_TB.Instance.ClearAllPools();
            //SceneManager.LoadScene("Map");
        }
        
    }

}