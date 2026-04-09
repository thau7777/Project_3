using Cysharp.Threading.Tasks;
using MyRule.DataService;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class GameSystemManager : PersistentSingleton<GameSystemManager>
    {
        // public static GameSystemManager Instance { get; private set; }
        private const string fileName = "/gamedata.json";
        [SerializeField] private bool encrypted = false;

        private GameData _gameData = new GameData();

        public GameData GameData => _gameData;

        private IDataService _dataService = new JsonDataService();

        private List<IGameData> datas = new();

        public bool HasSaveData = false;

        public bool IsLoadCompleted { get; private set; }

        protected override void Awake()
        {
            Debug.Log($"[GameSystemManager] Awake - instance null: {instance == null}, this: {GetInstanceID()}");
            base.Awake();
            Debug.Log($"[GameSystemManager] After base.Awake - instance ID: {instance.GetInstanceID()}");
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            LoadData().Forget();
        }

        #region Load, Save
        public async UniTask CreateNewGame()
        {
            _gameData = new GameData();

            foreach (var data in datas)
            {
                await data.NewGame();
            }
        }

        public async UniTask LoadData()
        {
            IsLoadCompleted = false;

            await UniTask.Yield(PlayerLoopTiming.Update);

            GameData gameDataLoaded = _dataService.LoadData<GameData>(fileName, encrypted);

            if (gameDataLoaded != null)
            {
                _gameData = gameDataLoaded;
                HasSaveData = true;
            }

            foreach (var data in datas)
            {
                data.LoadData(_gameData).Forget();
            }

            IsLoadCompleted = true;
        }

        public void SaveData()
        {
            foreach (var data in datas)
            {
                data.SaveData(_gameData);
            }

            _dataService.SaveData(fileName, _gameData, encrypted);
        }
        #endregion

        #region AutoSave
        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == Loader.EScene.LoadingScene.ToString()) return;

            await UniTask.WaitUntil(() => IsLoadCompleted);

            foreach (var data in datas)
            {
                data.LoadData(_gameData).Forget();
            }
        }
        #endregion

        #region Registration
        public void Register(IGameData data)
        {
            if (!datas.Contains(data))
            {
                datas.Add(data);
            }
        }

        public void Unregister(IGameData data)
        {
            datas.Remove(data);
        }
        #endregion

        private void OnApplicationQuit()
        {
            SaveData();
        }
    }
}