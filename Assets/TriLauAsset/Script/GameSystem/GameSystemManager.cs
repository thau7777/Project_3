using Cysharp.Threading.Tasks;
using MyRule.DataService;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class GameSystemManager : PersistentSingleton<GameSystemManager>
    {
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
            base.Awake();

            LoadData().Forget();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #region Load, Save
        public UniTask CreateNewGame()
        {
            _gameData = new GameData();

            return UniTask.CompletedTask;
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
                await data.LoadData(_gameData);
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
                await data.LoadData(_gameData);
            }
        }
        #endregion

        #region Registration
        public void Register(IGameData data)
        {
            if (!datas.Contains(data))
                datas.Add(data);
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