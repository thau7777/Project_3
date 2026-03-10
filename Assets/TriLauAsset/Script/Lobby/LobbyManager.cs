using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class Lobbymanager : MonoBehaviour, IGameData
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

        public UniTask LoadData(GameData data)
        {
            gold = data.LobbyData.Gold;
            crystal = data.LobbyData.Crystal;

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.LobbyData.SetGold(gold);
            data.LobbyData.SetCrystal(crystal);
        }
    }
}