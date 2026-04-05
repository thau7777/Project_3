using Cysharp.Threading.Tasks;
using MyRule.Event;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    public struct WeatherRate
    {
        public bool isBadWeather;
        public int rate;
    }

    [Serializable]
    public class WeatherData
    {
        [JsonProperty] private bool isBadWeather;

        [JsonIgnore] public bool IsBadWeather => isBadWeather;

        public WeatherData() 
        {

        }

        public void SetBadWeather(bool isBadWeather) => this.isBadWeather = isBadWeather;
    }

    public class WeatherManager : PersistentSingleton<WeatherManager>, IGameData
    {
        private WeatherData weatherData;

        private WeatherRate[] weatherRates;

        private EventBinding<ToolWeatherEvent> toolWeatherEvt;

        private bool GetRandomWeather()
        {
            int random = UnityEngine.Random.Range(1, 100);
            int rate = 0;

            for (int i = 0; i < weatherRates.Length; i++)
            {
                rate += weatherRates[i].rate;
                if (rate > random)
                {
                    return weatherRates[i].isBadWeather;
                }
            }
            
            return false;
        }

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);

            toolWeatherEvt = new EventBinding<ToolWeatherEvent>(SetBadWeatherInNextComabat);
            EventBus<ToolWeatherEvent>.Register(toolWeatherEvt);

            weatherRates = new WeatherRate[2]
            {
                new WeatherRate 
                {
                    isBadWeather = false,
                    rate = 70,
                },
                new WeatherRate 
                {
                    isBadWeather = true,
                    rate = 30,
                },
            };
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
            EventBus<ToolWeatherEvent>.Deregister(toolWeatherEvt);
        }

        private void SetWeather()
        {            
            EventBus<WeatherEvent>.Raise(new WeatherEvent(weatherData.IsBadWeather));
        }

        private void SetBadWeatherInNextComabat()
        {
            weatherData.SetBadWeather(true);
            SetWeather();
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData == null) return UniTask.CompletedTask;

            if (data.MatchData.WeatherData != null)
            {
                weatherData = data.MatchData.WeatherData;
            }
            else if (data.MatchData.WeatherData == null)
            {
                weatherData = new WeatherData();

                bool isbadWeather = GetRandomWeather();

                weatherData.SetBadWeather(isbadWeather);
            }

            SetWeather();

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData == null) return;

            data.MatchData.SetWeather(weatherData);
        }

        public UniTask NewGame()
        {
            weatherData = null;
            return UniTask.CompletedTask;
        }
    }
}