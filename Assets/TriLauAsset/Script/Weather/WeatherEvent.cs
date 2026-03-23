using UnityEngine;

namespace MyRule.Event
{
    public struct WeatherEvent : IEvent
    {
        public readonly EWeatherType weatherType;

        public WeatherEvent(EWeatherType weatherType)
        {
            this.weatherType = weatherType;
        }
    }
}