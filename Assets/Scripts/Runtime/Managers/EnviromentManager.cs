using Ami.BroAudio;
using Cysharp.Threading.Tasks;
using MyRule;
using System;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

public class EnviromentManager : Singleton<EnviromentManager>
{
    [Serializable]
    private struct MaterialWeatherEntry
    {
        public Material Mat;
        public string MetallicPropertyName;
        public string SmoothnessPropertyName;
        [Range(0, 1)] public float RainMetallic;
        [Range(0, 1)] public float RainSmoothness;

        public string MetallicProp => string.IsNullOrEmpty(MetallicPropertyName) ? "_Metallic" : MetallicPropertyName;
        public string SmoothnessProp => string.IsNullOrEmpty(SmoothnessPropertyName) ? "_Smoothness" : SmoothnessPropertyName;
    }

    [Serializable]
    private struct MapInfo
    {
        public EMap MapType;
        public GameObject MapPrefab;
        public Light WeatherMainLight;
        public Light BadWeatherMainLight;
        public GameObject BadWeatherExtraEffects;
        public List<MaterialWeatherEntry> WeatherMaterials; 
        public SoundID NormalAmbienceSound;
        public SoundID BadAmbienceSound;
    }

    [TabGroup("BasicMaps"), SerializeField] private List<MapInfo> _mapInfos;
    [TabGroup("SnowSpecialSettings"), SerializeField] private CameraCullingMaskController _maskController;
    [TabGroup("SnowSpecialSettings"), SerializeField] private Camera _mainCamera;
    [TabGroup("SnowSpecialSettings"), SerializeField] private LayerMask _skillIndicatorLayer;

    [TabGroup("SoundSettings"), SerializeField] private float _transitionDuration = default;


    [SerializeField] private bool _isBadWeatherForTest = true;
    private readonly Dictionary<Material, (float Metallic, float Smoothness)> _originalMaterialValues = new();
    private Light _currentLight;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        CacheAllOriginalMaterialValues();

        if (!TopDownGameManager.Instance.isTestGameplay)
        {
            UpdateMapType();
            return;
        }
        DisableAllMap();
        EnableMap(_mapInfos[0].MapType);
    }


    private void CacheAllOriginalMaterialValues()
    {
        foreach (var mapInfo in _mapInfos)
        {
            if (mapInfo.WeatherMaterials == null) continue;
            foreach (var entry in mapInfo.WeatherMaterials)
            {
                if (entry.Mat == null) continue;
                if (_originalMaterialValues.ContainsKey(entry.Mat)) continue;

                float metallic = entry.Mat.HasProperty(entry.MetallicProp) ? entry.Mat.GetFloat(entry.MetallicProp) : 0f;
                float smoothness = entry.Mat.HasProperty(entry.SmoothnessProp) ? entry.Mat.GetFloat(entry.SmoothnessProp) : 0f;
                _originalMaterialValues[entry.Mat] = (metallic, smoothness);
            }
        }
    }

    private async void UpdateMapType()
    {
        DisableAllMap();

        await UniTask.WaitUntil(() => MatchManager.Instance.MatchData != null);

        switch (MatchManager.Instance.MatchData.MapType)
        {
            case EMap.GreenLand: EnableMap(EMap.GreenLand); break;
            case EMap.Desert: EnableMap(EMap.Desert); break;
            case EMap.IceLand: EnableMap(EMap.IceLand); break;
        }
    }

    private void DisableAllMap()
    {
        foreach (var mapInfo in _mapInfos)
        {
            mapInfo.MapPrefab.SetActive(false);
            RestoreMapMaterials(mapInfo);
        }
    }

    private void EnableMap(EMap mapType)
    {
        if (mapType == EMap.IceLand)
        {
            _maskController.AddLayerToCulling(_skillIndicatorLayer);
            //remove that indicator layer from main
            _mainCamera.cullingMask &= ~_skillIndicatorLayer;
        }
        foreach (var mapInfo in _mapInfos)
        {
            if (mapInfo.MapType != mapType) continue;
            mapInfo.MapPrefab.SetActive(true);
            CheckWeather(mapInfo);
            return;
        }
    }

    private void CheckWeather(MapInfo map)
    {
        bool isBad = !TopDownGameManager.Instance.isTestGameplay ? MatchManager.Instance.MatchData.WeatherData.WeatherType == EWeatherType.Rain : _isBadWeatherForTest;
        _currentLight = isBad ? map.BadWeatherMainLight : map.WeatherMainLight;
        ApplyMapAmbienceSound(map, isBad);
        map.WeatherMainLight.gameObject.SetActive(!isBad);
        if (map.BadWeatherMainLight != null)
            map.BadWeatherMainLight.gameObject.SetActive(isBad);
        if (map.BadWeatherExtraEffects != null)
            map.BadWeatherExtraEffects.gameObject.SetActive(isBad);

        if (isBad)
            ApplyRainMaterials(map);
        else
            RestoreMapMaterials(map);

        if(isBad && map.MapType == EMap.GreenLand)
            LightningLightController.Instance.StartLightningLoop();
    }
    private void ApplyMapAmbienceSound(MapInfo mapInfo, bool isBad)
    {
        SoundID sound = isBad ? mapInfo.BadAmbienceSound : mapInfo.NormalAmbienceSound;
        BroAudio.Play(sound, _transitionDuration);
    }

    private void ApplyRainMaterials(MapInfo map)
    {
        if (map.WeatherMaterials == null) return;
        foreach (var entry in map.WeatherMaterials)
        {
            if (entry.Mat == null) continue;
            if (entry.Mat.HasProperty(entry.MetallicProp))
                entry.Mat.SetFloat(entry.MetallicProp, entry.RainMetallic);
            if (entry.Mat.HasProperty(entry.SmoothnessProp))
                entry.Mat.SetFloat(entry.SmoothnessProp, entry.RainSmoothness);
        }
    }

    private void RestoreMapMaterials(MapInfo map)
    {
        if (map.WeatherMaterials == null) return;
        foreach (var entry in map.WeatherMaterials)
        {
            if (entry.Mat == null) continue;
            if (!_originalMaterialValues.TryGetValue(entry.Mat, out var og)) continue;

            if (entry.Mat.HasProperty(entry.MetallicProp))
                entry.Mat.SetFloat(entry.MetallicProp, og.Metallic);
            if (entry.Mat.HasProperty(entry.SmoothnessProp))
                entry.Mat.SetFloat(entry.SmoothnessProp, og.Smoothness);
        }
    }

    private void OnDisable()
    {
        foreach (var mapInfo in _mapInfos)
            RestoreMapMaterials(mapInfo);
        BroAudio.Stop(BroAudioType.Ambience);

    }

    public async UniTaskVoid LerpLightIntensity(float duration, float holdDuration)
    {
        if (_currentLight == null) return;

        float originalIntensity = _currentLight.intensity;

        // Lerp to 0
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _currentLight.intensity = Mathf.Lerp(originalIntensity, 0f, elapsed / duration);
            await UniTask.Yield();
        }
        _currentLight.intensity = 0f;

        // Hold
        await UniTask.Delay((int)(holdDuration * 1000));

        // Lerp back
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _currentLight.intensity = Mathf.Lerp(0f, originalIntensity, elapsed / duration);
            await UniTask.Yield();
        }
        _currentLight.intensity = originalIntensity;
    }
}