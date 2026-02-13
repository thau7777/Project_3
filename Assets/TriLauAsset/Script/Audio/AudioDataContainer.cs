using System.Collections.Generic;
using UnityEngine;

namespace MyRule.Audio 
{
    [CreateAssetMenu(fileName = "New AudioDataContainer", menuName = "Scriptable Objects/Audio Data Container")]
    public class AudioDataContainer : ScriptableObject
    {
        public List<SFXData> sfxList;
        public List<MusicData> musicList;
    }
}

