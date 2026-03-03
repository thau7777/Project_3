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
        Skill_Impact,
        Hurt,
        EnemyHurt,
        EnemySound,



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
    public class SFXData
    {
        public SFXType sfxType;
        public List<AudioClip> clips;

        public bool random;
        [MinMaxSlider(-1f, 1f)] public Vector2 pitchRandom;
    }

    [System.Serializable]
    public class MusicData
    {
        public MusicType musicType;
        public AudioClip clip;
    }

    
}