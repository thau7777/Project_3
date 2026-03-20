using UnityEngine;

namespace MyRule
{
    public class LandManager : MonoBehaviour
    {
        private async void Start()
        {
            await Loader.LoadSceneAdditive(Loader.EScene.MazeScene);
        }
    }
}