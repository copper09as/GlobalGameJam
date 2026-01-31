using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class AudioSystem : IGameSystem
{
    private float masterVolume = 1f; // 0–1
    private float bgmVolume = 1f;    // 0–1
    private float sfxVolume = 1f;    // 0–1
    public int Priority => 0;
    private AudioSource bgmAudioSource;
    private List<AudioSource> sfxAudioSourceList = new List<AudioSource>();
    private string currentAudio;
    private string currentBgmName;
    private AudioMixer audioMixer;
    private const string AudioMixerPath = "Audio/AudioMixer.asset";
    private const string AudioDataBasePath = "Assets/Audio/AudioDataBase.asset";
    private  AudioDataBase audioDataBase;
    private const int MaxSfxAudioSourceCount = 5;
    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
    public void OnInit()
    {

        audioMixer = Addressables
            .LoadAssetAsync<AudioMixer>(AudioMixerPath)
            .WaitForCompletion();
        audioDataBase = Addressables
            .LoadAssetAsync<AudioDataBase>(AudioDataBasePath)
            .WaitForCompletion();
        bgmAudioSource = CreateAudioSource("BGMAudioSource", audioMixer.FindMatchingGroups("Bgm")[0]);
        bgmAudioSource.loop = true;
        for (int i = 0; i < MaxSfxAudioSourceCount; i++)
        {
            AudioSource sfxAudioSource = CreateAudioSource($"SFXAudioSource_{i}", audioMixer.FindMatchingGroups("Sfx")[0]);
            sfxAudioSourceList.Add(sfxAudioSource);
            sfxAudioSource.loop = false;
        }
        SetMasterVolume(PlayerPrefs.GetFloat("MasterSound", 50f));
        SetBgmVolume(PlayerPrefs.GetFloat("BgmSound", 50f));
        SetSfxVolume(PlayerPrefs.GetFloat("SfxSound", 50f));
    }

    private AudioSource CreateAudioSource(string audioName, AudioMixerGroup outputMixerGroup)
    {
        GameObject bgmObject = new GameObject(audioName);
        bgmObject.transform.SetParent(GameEntry.Instance.transform);
        AudioSource audioSource = bgmObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = outputMixerGroup;
        return audioSource;
    }
    public void OnShutdown()
    {
        Debug.Log("[AudioSystem] 关闭完成");
    }
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value / 100f);
        UpdateVolumes();
        PlayerPrefs.SetFloat("MasterSound", value);
    }

    public void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value / 100f);
        UpdateVolumes();
        PlayerPrefs.SetFloat("BgmSound", value);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value / 100f);
        UpdateVolumes();
        PlayerPrefs.SetFloat("SfxSound", value);
    }
    private void UpdateVolumes()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = masterVolume * bgmVolume;
        }

        foreach (var sfx in sfxAudioSourceList)
        {
            if (sfx != null)
            {
                sfx.volume = masterVolume * sfxVolume;
            }
        }
    }

    public void OnUpdate(float deltaTime)
    {
    }
    public void PlayBGMByName(string name)
    {
        if (name == currentBgmName) return;
        var audioData = audioDataBase.audioDataList.Find(data => data.audioName == name);
        if (audioData != null)
        {
            PlayBGM(audioData.audioClip);
            currentBgmName = name;
        }
        else
        {
            Debug.LogError($"[AudioSystem] 未找到名称为{name}的BGM");
        }
    }
    public void PlaySFXByName(string name)
    {
        var audioData = audioDataBase.audioDataList.Find(data => data.audioName == name);
        if (audioData != null)
        {
            PlaySFX(audioData.audioClip);
           
        }
        else
        {
            Debug.LogError($"[AudioSystem] 未找到名称为{name}的SFX");
        }
    }
    public void PlayBGM(string clipName)
    {
        if (currentAudio == clipName) return;
        if (!audioClipCache.TryGetValue(clipName, out AudioClip cachedClip))
        {
            cachedClip = Addressables
                .LoadAssetAsync<AudioClip>(clipName)
                .WaitForCompletion();
            audioClipCache.Add(clipName, cachedClip);
        }
        PlayBGM(cachedClip);
        currentAudio = clipName;
    }
    public void PlaySFX(string clipName)
    {
        if (!audioClipCache.TryGetValue(clipName, out AudioClip cachedClip))
        {
            cachedClip = Addressables
                .LoadAssetAsync<AudioClip>(clipName)
                .WaitForCompletion();
            audioClipCache.Add(clipName, cachedClip);
        }
        PlaySFX(cachedClip);

    }
    private void PlayBGM(AudioClip clip)
    {
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }
    private void PlaySFX(AudioClip clip)
    {
        for (int i = 0; i < sfxAudioSourceList.Count; i++)
        {
            var source = sfxAudioSourceList[i];

            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                return;
            }
        }

        // 如果全在播，直接替换第一个
        var firstSource = sfxAudioSourceList[0];
        firstSource.Stop();
        firstSource.clip = clip;
        firstSource.Play();
    }
}