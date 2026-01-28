using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AudioDataBase", menuName = "ScriptableObjects/Audio/AudioDataBase", order = 1)]
public class AudioDataBase : ScriptableObject
{
    public List<AudioData> audioDataList = new List<AudioData>();
}
[System.Serializable]
public class AudioData
{
    public string audioName;
    public AudioClip audioClip;
}