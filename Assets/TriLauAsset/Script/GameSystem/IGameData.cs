using Cysharp.Threading.Tasks;

namespace MyRule
{
    public interface IGameData
    {
        UniTask NewGame();
        UniTask LoadData(GameData data);
        void SaveData(GameData data);
    }
}