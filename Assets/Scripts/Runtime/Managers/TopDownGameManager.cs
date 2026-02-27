using Cysharp.Threading.Tasks;
using UnityEngine;
public class TopDownGameManager : Singleton<TopDownGameManager>
{
    private EventBinding<TopDownPlayerDeadEvent> _topDownPlayerDeadEventBinding;
    private void OnEnable()
    {
        _topDownPlayerDeadEventBinding = new EventBinding<TopDownPlayerDeadEvent>(OnPlayerDeath);
        EventBus<TopDownPlayerDeadEvent>.Register(_topDownPlayerDeadEventBinding);
    }
    private void OnDisable()
    {
        EventBus<TopDownPlayerDeadEvent>.Deregister(_topDownPlayerDeadEventBinding);

    }
    private void Start()
    {
        StartMatch();

    }

    private async void StartMatch()
    {
        await UniTask.Delay(2000);
        EventBus<TopDownStartGameEvent>.Raise(new TopDownStartGameEvent());
    }

    private async void OnPlayerDeath()
    {
        Time.timeScale = 0;
        await UniTask.Delay(500,true);
        Time.timeScale = 1;
    }
}
