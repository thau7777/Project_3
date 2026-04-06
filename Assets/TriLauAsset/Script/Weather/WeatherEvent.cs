using UnityEngine;

namespace MyRule.Event
{
    public struct WeatherEvent : IEvent
    {
        public readonly bool isBadWeather;

        public WeatherEvent(bool weatherType)
        {
            isBadWeather = weatherType;
        }
    }
}