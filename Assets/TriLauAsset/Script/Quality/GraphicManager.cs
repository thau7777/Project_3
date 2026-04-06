using UnityEngine;

namespace MyRule
{
    public class GraphicManager : PersistentSingleton<GraphicManager>
    {
        private int currentResolutionIndex;
        private bool isFullScreen = true;

        private const string RESOLUTION_INDEX_KEY = "ScreenResolution";
        private const string FULLSCREEN_KEY = "DisplayMode";

        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        private void Init()
        {
            currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, 0);
            isFullScreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 0) == 0;

            ApplyResolution(currentResolutionIndex, isFullScreen);
        }

        public void SetResolution(int index)
        {
            ApplyResolution(index, isFullScreen);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            this.isFullScreen = isFullscreen;
            Debug.Log("Full screen " + isFullscreen);
            ApplyResolution(currentResolutionIndex, isFullscreen); 
        }

        private void ApplyResolution(int index, bool isFullscreen)
        {
            switch (index)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, isFullscreen);
                    Debug.Log("Resolution 1920x1080 " + Screen.fullScreen);
                    break;
                case 1:
                    Screen.SetResolution(2560, 1440, isFullscreen);
                    Debug.Log("Resolution 2560x1440 " + Screen.fullScreen);
                    break;
                case 2:
                    Screen.SetResolution(1600, 900, isFullscreen);
                    Debug.Log("Resolution 1600x900 " + Screen.fullScreen);
                    break;
                case 3:
                    Screen.SetResolution(1600, 1000, isFullscreen);
                    Debug.Log("Resolution 1600x1000 " + Screen.fullScreen);
                    break;
            }

            this.isFullScreen = isFullscreen;
            this.currentResolutionIndex = index;
        }
    }
}