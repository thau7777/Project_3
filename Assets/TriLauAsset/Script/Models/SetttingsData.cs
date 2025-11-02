using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "SetttingsData", menuName = "Game/Settings/SetttingsData")]
    public class SettingsData : ScriptableObject
    {
        [Header("General")]
        public string language = "English";

        [Header("Graphics")]
        public bool fullscreen = true;
        public int resolutionIndex = 0;
        public int qualityIndex = 1;

        [Header("Sound")]
        public float masterVolume = 1.0f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 0.8f;

        [Header("Controls")]
        public float mouseSensitivity = 1.0f;
        public bool invertY = false;
    }
}