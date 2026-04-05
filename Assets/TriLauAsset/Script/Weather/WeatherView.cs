using Cysharp.Threading.Tasks;
using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class WeatherView : MonoBehaviour
    {
        [SerializeField] private GameObject weatherObj;

        [SerializeField] private Light directionLight;

        [SerializeField] private Color rainColor;
        [SerializeField] private Color snowColor;

        private EventBinding<WeatherEvent> weatherEventBinding;

        private void OnEnable()
        {
            weatherEventBinding = new EventBinding<WeatherEvent>(HandleWeather);
            EventBus<WeatherEvent>.Register(weatherEventBinding);
        }

        private void OnDisable()
        {
            EventBus<WeatherEvent>.Deregister(weatherEventBinding);
        }

        private void Start()
        {
            SetRandomWeather();
        }

        private async void SetRandomWeather()
        {
            await UniTask.WaitUntil(() => WeatherManager.Instance.WeatherData != null);

            bool isbadWeather = WeatherManager.Instance.GetRandomWeather();

            WeatherManager.Instance.WeatherData.SetBadWeather(isbadWeather);

            WeatherManager.Instance.SetWeather();
        }    

        private void HandleWeather(WeatherEvent evt)
        {
            if (evt.isBadWeather)
            {
                if (weatherObj != null) weatherObj.SetActive(true);
            }
        }
    }
}