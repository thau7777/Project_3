using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Turnbase
{
    public class TB_AudioSkillManager : MonoBehaviour
    {
        public static TB_AudioSkillManager Instance;

        [Header("Settings")]
        [SerializeField] private AudioMixerGroup sfxGroup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }
        }

        public void PlaySkillSound(Skill.SkillSound skillSound)
        {
            if (skillSound.clip == null) return;

            GameObject soundObj = new GameObject("Temp_SkillSound_" + skillSound.clip.name);
            AudioSource source = soundObj.AddComponent<AudioSource>();

            source.clip = skillSound.clip;
            source.outputAudioMixerGroup = sfxGroup;
            source.volume = skillSound.volume;

            float randomPitch = Random.Range(-0.05f, 0.05f);
            source.pitch = skillSound.pitch + randomPitch;

            source.spatialBlend = 0f;

            source.Play();

            Destroy(soundObj, skillSound.clip.length);
        }
    }
}