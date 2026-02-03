using Cysharp.Threading.Tasks;
using UnityEngine;
public class TopDownGameManager : Singleton<TopDownGameManager>
{
    private void Start()
    {
        StartMatch();

    }

    private async void StartMatch()
    {
        await UniTask.Delay(2000);
        EventBus<TopDownStartGameEvent>.Raise(new TopDownStartGameEvent());
    }
}
