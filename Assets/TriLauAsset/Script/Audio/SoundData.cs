using System.Collections.Generic;
using UnityEngine;

namespace MyRule.Audio
{
    public enum SFXType
    {
        Shoot,
        Explosion,
        GemPickup,
        Hit,
        UI_Click,
        UI_Select,
        UI_Adjust,
    }

    public enum MusicType
    {
        MainMenu,
        Gameplay,
        Boss,
        Victory,
        GameOver
    }

    [System.Serializable]
    public class SoundData
    {
        public SFXType sfxType;
        public List<AudioClip> clips;

        public bool random;
        [Range(0f, 0.2f)] public float pitchRandom;
    }

    [System.Serializable]
    public class MusicData
    {
        public MusicType musicType;
        public AudioClip clip;
    }
}