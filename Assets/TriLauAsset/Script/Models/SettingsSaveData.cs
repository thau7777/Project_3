namespace MyRule
{
    [System.Serializable]
    public class SettingsSaveData
    {
        public string language;
        public bool fullscreen;
        public int resolutionIndex;
        public int qualityIndex;

        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;

        public float mouseSensitivity;
        public bool invertY;
    }
}