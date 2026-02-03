using MyRule.Audio;
using UnityEngine;

namespace MyRule
{
    public class MainMenuManager : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.Instance.PlayMusic(MusicType.MainMenu);
        }
    }
}