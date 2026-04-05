using Ami.BroAudio;
using UnityEngine;

public class SoundChainLightning : MonoBehaviour
{
    public SoundID sound1;
    public SoundID sound2;
    public SoundID sound3;

    public void PlayChainLightning()
    {
        BroAudio.Play(sound1);
        BroAudio.Play(sound2);
        BroAudio.Play(sound3);
    }

}
