using UnityEngine;
using MyRule;
using Cysharp.Threading.Tasks;
using Ami.BroAudio;

namespace Turnbase
{
    public class TerrianMap : MonoBehaviour
    {
        public GameObject[] terrain;

        public GameObject[] weather;

        public Transform[] transforms;

        public SoundID battle;

        public SoundID greenLand;
        public SoundID desertLand;
        public SoundID iceLand;

        private void Start()
        {
            SwichTerrain();
            Weather();

            BroAudio.Play(battle);
        }

        private void OnDisable()
        {
            BroAudio.Stop(battle);
        }

        public async void SwichTerrain()
        {
            await UniTask.WaitUntil(() => MatchManager.Instance.MatchData != null);

            EMap mapType = MatchManager.Instance.MatchData.MapType;

            switch (mapType)
            {
                case EMap.GreenLand:
                    terrain[0].gameObject.SetActive(true);
                    terrain[1].gameObject.SetActive(false);
                    terrain[2].gameObject.SetActive(false);

                    BroAudio.Play(greenLand);

                    if (transforms.Length > 0 && transforms[0] != null)
                    {
                        terrain[0].transform.position = transforms[0].position;
                        terrain[0].transform.rotation = transforms[0].rotation;
                    }
                    break;

                case EMap.Desert:
                    terrain[1].gameObject.SetActive(true);
                    terrain[0].gameObject.SetActive(false);
                    terrain[2].gameObject.SetActive(false);

                    BroAudio.Play(desertLand);

                    if (transforms.Length > 1 && transforms[1] != null)
                    {
                        terrain[1].transform.position = transforms[1].position;
                        terrain[1].transform.rotation = transforms[1].rotation;
                    }
                    break;

                case EMap.IceLand:
                    terrain[2].gameObject.SetActive(true);
                    terrain[1].gameObject.SetActive(false);
                    terrain[0].gameObject.SetActive(false);

                    BroAudio.Play(iceLand);

                    if (transforms.Length > 2 && transforms[2] != null)
                    {
                        terrain[2].transform.position = transforms[2].position;
                        terrain[2].transform.rotation = transforms[2].rotation;
                    }
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
                    if (weather[0] != null) weather[0].gameObject.SetActive(true);
                    break;

                case EWeatherType.Snow:
                    if (weather[1] != null) weather[1].gameObject.SetActive(true);
                    break;
            }
        }
    }
}