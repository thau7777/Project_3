using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MyRule.Audio
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource[] sfxSources;

        [Header("Audio Data")]
        [SerializeField] private AudioDataContainer _audioDataContainer;

        private Dictionary<SFXType, SFXData> sfxDict;
        private Dictionary<SFXType, int> sfxIndexDict;
        private Dictionary<MusicType, AudioClip> musicDict;

        private int sfxSourceIndex;

        // ====================== LIFECYCLE ======================
        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        private void Init()
        {
            sfxDict = new Dictionary<SFXType, SFXData>();
            sfxIndexDict = new Dictionary<SFXType, int>();
            musicDict = new Dictionary<MusicType, AudioClip>();

            foreach (var sfx in _audioDataContainer.sfxList)
            {
                if (sfx.clips == null || sfx.clips.Count == 0)
                    continue;

                sfxDict[sfx.sfxType] = sfx;
                sfxIndexDict[sfx.sfxType] = 0;
            }

            foreach (var music in _audioDataContainer.musicList)
            {
                musicDict[music.musicType] = music.clip;
            }
        }

        // ====================== SFX ======================
        public void PlaySFX(SFXType type)
        {
            if (!sfxDict.ContainsKey(type)) return;

            SFXData data = sfxDict[type];
            AudioClip clip = GetNextClip(type, data);

            AudioSource source = sfxSources[sfxSourceIndex];
            sfxSourceIndex = (sfxSourceIndex + 1) % sfxSources.Length;

            source.pitch = Random.Range(data.pitchRandom.x, data.pitchRandom.y);
            source.PlayOneShot(clip);
        }

        private AudioClip GetNextClip(SFXType type, SFXData data)
        {
            if (data.random && data.clips.Count > 1)
            {
                int newIndex;
                int current = sfxIndexDict[type];

                do
                {
                    newIndex = Random.Range(0, data.clips.Count);
                }
                while (newIndex == current);

                sfxIndexDict[type] = newIndex;
                return data.clips[newIndex];
            }
            else
            {
                int index = sfxIndexDict[type];
                AudioClip clip = data.clips[index];

                index = (index + 1) % data.clips.Count;
                sfxIndexDict[type] = index;

                return clip;
            }
        }

        // ====================== MUSIC ======================
        public void PlayMusic(MusicType type, bool loop = true)
        {
            if (!musicDict.ContainsKey(type)) return;

            if (musicSource.clip == musicDict[type]) return;

            musicSource.clip = musicDict[type];
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        // ====================== MIXER ======================
        public void SetGeneralVolume(float value)
        {
            audioMixer.SetFloat("GeneralVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        }

        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        }
    }
}