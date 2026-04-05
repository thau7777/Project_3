using UnityEngine;
using UnityEngine.UI;
using MyRule.Audio;

namespace MyRule.UI
{
    public class SoundSettingSliderView : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            AudioManager.Instance.LoadSoundSettings();
            masterSlider.value = AudioManager.Instance.masterVolume;
            musicSlider.value = AudioManager.Instance.musicVolume;
            sfxSlider.value = AudioManager.Instance.sfxVolume;
        }

        private void OnDestroy()
        {
            AudioManager.Instance.SaveSoundSettings(masterSlider.value, musicSlider.value, sfxSlider.value);
        }
    }
}