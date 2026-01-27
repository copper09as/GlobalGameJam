using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public void MasterAudioSoureChanged(float value)
    {
        GameEntry.Instance.GetSystem<AudioSystem>().SetMasterVolume(value);
    }
    public void BgmAudioSoureChanged(float value)
    {
        GameEntry.Instance.GetSystem<AudioSystem>().SetBgmVolume(value);
    }
    public void SfxAudioSoureChanged(float value)
    {
        GameEntry.Instance.GetSystem<AudioSystem>().SetSfxVolume(value);
    }
}
