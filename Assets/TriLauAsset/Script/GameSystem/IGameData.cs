using Cysharp.Threading.Tasks;

namespace MyRule
{
    public interface IGameData
    {
        UniTask LoadData(GameData data);
        void SaveData(GameData data);
    }
}