using Cysharp.Threading.Tasks;
using MyRule.CommandPattern;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace MyRule
{
    public class PortalManager : Singleton<PortalManager>
    {
        [SerializeField] private GameObject keyTxt;
        [SerializeField] private PlayableDirector portalTimeline;
        [SerializeField] private PlayableDirector startGameTimeline;
        [SerializeField] private Button startBtn;
        private Loader.EScene targetScene;
        
        [SerializeField] private MeshRenderer portalRenderer;
        [SerializeField] private float cutscene1Duration = 3f;
        [SerializeField] private float cutscene2Duration = 0.8f;

        private bool hasTargetScene = false;
        private bool canInteract = false;

        public bool CanInteract
        {
            get => canInteract;
            set
            {
                canInteract = value;
                Debug.Log("Portal can interact: " + canInteract);
            }
        }

        private void OnEnable()
        {
            startBtn.onClick.AddListener(OnStartBtnClicked);
        }

        private void OnDisable()
        {
            startBtn.onClick.RemoveListener(OnStartBtnClicked);
        }

        private void Start()
        {
            portalRenderer.material.SetFloat("_Indestry", 2f);
        }

        public void SetTargetScene(Loader.EScene scene)
        {
            targetScene = scene;
            hasTargetScene = true;
            portalTimeline.Play();

            HighlightPortalAsync(200f, cutscene1Duration).Forget();
        }

        private async UniTask HighlightPortalAsync(float to, float duration)
        {
            float time = 0f;
            float from = portalRenderer.material.GetFloat("_Indestry");

            while (time < duration)
            {
                float intensity = Mathf.Lerp(from, to, time / duration);
                portalRenderer.material.SetFloat("_Indestry", intensity);

                time += Time.deltaTime;
                await UniTask.Yield();
            }

            portalRenderer.material.SetFloat("_Indestry", to);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && hasTargetScene)
            {
                CanInteract = true;
                keyTxt.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.CompareTag("Player"))
            {
                CanInteract = false;
                keyTxt.SetActive(false);
            }
        }

        public async void OnStartBtnClicked()
        {
            if (canInteract)
            {
                CommandInvoker.UndoCommand();

                await UniTask.Delay(200);

                startGameTimeline.Play();

                VolumeController.Instance.AdjustFlareVolumeWeight();

                HighlightPortalAsync(5000f, cutscene2Duration).Forget();

                await UniTask.Delay(7000);
                
                Cursor.lockState = CursorLockMode.None;

                Loader.Load(targetScene, Loader.ELoadMode.WithLoadingScreen);
            }
        }
    }
}