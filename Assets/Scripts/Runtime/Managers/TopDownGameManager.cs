using Cysharp.Threading.Tasks;
using UnityEngine;
public class TopDownGameManager : Singleton<TopDownGameManager>
{
    private EventBinding<TopDownEndGameEvent> _topDownPlayerDeadEventBinding;
    private void OnEnable()
    {
        _topDownPlayerDeadEventBinding = new EventBinding<TopDownEndGameEvent>(OnPlayerDeath);
        EventBus<TopDownEndGameEvent>.Register(_topDownPlayerDeadEventBinding);
    }
    private void OnDisable()
    {
        EventBus<TopDownEndGameEvent>.Deregister(_topDownPlayerDeadEventBinding);

    }
    private void Start()
    {
        StartMatch();

    }

    private async void StartMatch()
    {
        await UniTask.Delay(2000);
        EventBus<TopdownStartGameEvent>.Raise(new TopdownStartGameEvent());
    }

    private async void OnPlayerDeath(TopDownEndGameEvent topDownEndGameEvent)
    {
        if (topDownEndGameEvent.endGameExecuteState != UIEndGameExecuteState.Lose) return;
        Time.timeScale = 0;
        await UniTask.Delay(500,true);
        Time.timeScale = 1;
    }
}
