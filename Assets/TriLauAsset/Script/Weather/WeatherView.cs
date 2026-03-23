using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class WeatherView : MonoBehaviour
    {
        [SerializeField] private GameObject rainObj;
        [SerializeField] private GameObject snowObj;

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

        private void HandleWeather(WeatherEvent evt)
        {
            switch (evt.weatherType)
            {
                case EWeatherType.Rain:
                    {
                        if (rainObj != null) rainObj.SetActive(true);
                        directionLight.color = rainColor;
                        break;
                    }
                case EWeatherType.Snow:
                    {
                        if (snowObj != null) snowObj.SetActive(true);
                        directionLight.color = snowColor;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}