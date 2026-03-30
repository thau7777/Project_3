using Cysharp.Threading.Tasks;
using MyRule.Event;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    public enum EWeatherType
    {
        None,
        Rain,
        Snow,
    }

    [Serializable]
    public class WeatherData
    {
        [JsonProperty] private EWeatherType type;

        [JsonIgnore] public EWeatherType WeatherType => type;

        public WeatherData() 
        {

        }

        public void SetWeatherType(EWeatherType type) => this.type = type;
    }

    public class WeatherManager : PersistentSingleton<WeatherManager>, IGameData
    {
        private WeatherData weatherData;

        private EWeatherType GetRandomWeather(EWeatherType eWeatherType1, EWeatherType eWeatherType2)
        {
            int random = UnityEngine.Random.Range(1, 3);

            if (random == 1) return eWeatherType1;
            else if (random == 2) return eWeatherType2;
            else return EWeatherType.None;
        }

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        private void SetWeather()
        {
            if (weatherData.WeatherType == EWeatherType.None) return;
            
            EventBus<WeatherEvent>.Raise(new WeatherEvent(weatherData.WeatherType));
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData.WeatherData != null)
            {
                weatherData = data.MatchData.WeatherData;
            }
            else if (data.MatchData.WeatherData == null)
            {
                weatherData = new WeatherData();

                if (data.MatchData.MapType == EMap.GreenLand)
                {
                    //EWeatherType weatherType = GetRandomWeather(EWeatherType.None, EWeatherType.Rain);
                    weatherData.SetWeatherType(EWeatherType.Rain);
                }
                else if (data.MatchData.MapType == EMap.Desert)
                {
                    weatherData.SetWeatherType(EWeatherType.None);
                }
                else if (data.MatchData.MapType == EMap.IceLand)
                {
                    EWeatherType weatherType = GetRandomWeather(EWeatherType.None, EWeatherType.Snow);
                    weatherData.SetWeatherType(weatherType);
                }
            }

            SetWeather();

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.MatchData.SetWeather(weatherData);
        }
    }
}