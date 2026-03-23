using Cysharp.Threading.Tasks;
using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class Lobbymanager : Singleton<Lobbymanager>, IGameData
    {
        [SerializeField] int gold;
        [SerializeField] int crystal;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void IncreaseGold(int amount)
        {
            gold += amount;

            UpdateGoldUI();
        }

        public void DecreaseGold(int amount)
        {
            gold -= amount;

            UpdateGoldUI();
        }

        public void IncreaseCrystal(int amount)
        {
            crystal += amount;

            UpdateCrystalUI();
        }

        public void DecreaseCrystal(int amount)
        {
            crystal -= amount;

            UpdateCrystalUI();
        }

        private void UpdateGoldUI()
        {
            EventBus<UpdateLobbyGoldUIEvent>.Raise(new UpdateLobbyGoldUIEvent(gold));
        }

        private void UpdateCrystalUI()
        {
            EventBus<UpdateLobbyCrystalUIEvent>.Raise(new UpdateLobbyCrystalUIEvent(crystal));
        }

        public UniTask LoadData(GameData data)
        {
            gold = data.LobbyData.Gold;
            crystal = data.LobbyData.Crystal;

            UpdateGoldUI();
            UpdateCrystalUI();

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.LobbyData.SetGold(gold);
            data.LobbyData.SetCrystal(crystal);
        }
    }
}