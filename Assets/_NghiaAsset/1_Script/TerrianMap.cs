using UnityEngine;
using MyRule;
using Cysharp.Threading.Tasks;

namespace Turnbase
{
    public class TerrianMap : MonoBehaviour
    {
        public GameObject[] terrain;

        public GameObject[] weather;

        private void Start()
        {
            SwichTerrain();
            Weather();
        }

        public async void SwichTerrain()
        {
            await UniTask.WaitUntil(() => MatchManager.Instance.MatchData != null);

            EMap mapType = MatchManager.Instance.MatchData.MapType;

            switch (mapType)
            {
                case EMap.GreenLand:
                    terrain[0].gameObject.SetActive(true);
                    break;

                case EMap.Desert:
                    terrain[1].gameObject.SetActive(true);

                    break;

                case EMap.IceLand:
                    terrain[2].gameObject.SetActive(true);
                    break;


            }

        }

        public async void Weather()
        {
            await UniTask.WaitUntil(() => MatchManager.Instance.MatchData != null);

            EWeatherType mapType = MatchManager.Instance.MatchData.WeatherData.WeatherType;

            switch (mapType)
            {

                case EWeatherType.Rain:
                    if (weather[0]!= null) weather[0].gameObject.SetActive(true);

                    break;

                case EWeatherType.Snow:
                    if (weather[1] != null) weather[1].gameObject.SetActive(true);
                    break;


            }

        }
    }

}
