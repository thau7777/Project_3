using Ami.BroAudio;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManualPlayer : MonoBehaviour
{
    [Serializable]
    private struct SoundToPlay
    {
        public SoundID SoundID;
        public float Delay;
        public bool AsDominator;
    }

    [SerializeField] private List<SoundToPlay> _soundList = new();

    private void OnEnable()
    {
        foreach (var sound in _soundList)
            PlayWithDelay(sound).Forget();
    }

    private async UniTaskVoid PlayWithDelay(SoundToPlay sound)
    {
        if (sound.Delay > 0f)
            await UniTask.Delay((int)(sound.Delay * 1000));

        if (sound.AsDominator)
            BroAudio.Play(sound.SoundID).AsDominator();
        else
            BroAudio.Play(sound.SoundID);
    }
}