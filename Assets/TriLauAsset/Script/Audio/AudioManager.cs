using Ami.BroAudio;
using Ami.BroAudio.Runtime;
using System;
using UnityEngine;

namespace MyRule.Audio
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private SoundID[] soundIDStorage;

        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        private void Start()
        {
            LoadSoundSettings();
        }

        public void PlaySound(string soundID)
        {
            for (int i = 0; i < soundIDStorage.Length; i++)
            {
                if (soundIDStorage[i].ToString().Equals(soundID))
                {
                    BroAudio.Play(soundIDStorage[i]);
                    return;
                }
            }
        }

        public void LoadSoundSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("Master", 1f);
            BroAudio.SetVolume(BroAudioType.All, masterVolume);

            musicVolume = PlayerPrefs.GetFloat("Music", 1f);
            BroAudio.SetVolume(BroAudioType.Music, musicVolume);

            sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);
            BroAudio.SetVolume(BroAudioType.SFX, sfxVolume);
        }

        public void SaveSoundSettings(float master = 1f, float music = 1f, float sfx = 1f)
        {
            PlayerPrefs.SetFloat("Master", master);
            PlayerPrefs.SetFloat("Music", music);
            PlayerPrefs.SetFloat("SFX", sfx);
        }
    }
}