using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static SoundManager;

[Serializable]
public struct BGMAudioData
{
    public EBGM BGMType;
    public AudioClip AudioClip;
}

[Serializable]
public struct SFXAudioData
{
    public ESFX SFXType;
    public AudioClip AudioClip;
}

[Serializable]
public struct SoundSettingData
{
    public float BGMVolume;
    public float SFXVolume;
}


public class SoundManager : SingletonObject<SoundManager>
{
    //준비 브금이 끝나기 전에 미리 틀어야 하는 시간
    public float StartSFXOffset = 0.25f;

    public enum EBGM
    {
        BGM_START,
        BGM_PLAYING,
        BGM_GAMEOVER,
        BGM_FEVER,
    }

    public enum ESFX
    {
        SFX_START,
        SFX_BUTTON,
        SFX_GROW,
        SFX_SHRINK,
        SFX_GAMEOVER,
        SFX_EAT,
        SFX_UNLOCKED,
    }

    [SerializeField] BGMAudioData[] BGMDataList;
    [SerializeField] SFXAudioData[] SFXDataList;

    public AudioSource BGMAudioSource;
    public AudioSource SFXAudioSource;

    private AudioClip LastBGM;
    private List<AudioClip> BGMQueue;

    public UnityEvent<EBGM> OnBGMChanged;
    public UnityEvent<ESFX> OnSFXChanged;

    public SoundSettingData SoundSettingData;

    private bool IsBGMNeedToPlay = true;

    protected override void Awake()
    {
        base.Awake();

        SaveSystem.Instance.OnSaveDataLoadedEvent.AddListener(OnSaveDataLoaded);
    }

    void Start()
    {
        BGMQueue = new List<AudioClip>();

        StartCoroutine(WaitNextBGMLoop());
    }

    private void OnSaveDataLoaded(SaveData LoadedSaveData)
    {
        BGMAudioSource.volume = SoundSettingData.BGMVolume = LoadedSaveData.BGMVolume;
        SFXAudioSource.volume = SoundSettingData.SFXVolume = LoadedSaveData.SFXVolume;
    }

    public void PlayBGM(EBGM BGMIndex, bool PlayNow)
    {
        PlayBGM(BGMDataList[(int)BGMIndex].AudioClip, PlayNow, false);
    }

    public void ReadyToStart()
    {
        StopCoroutine(ReadyToStart_Internal());
        StartCoroutine(ReadyToStart_Internal());
    }

    public void PlayBGM(AudioClip AudioClip, bool PlayNow, bool ForceReplay)
    {
        if(BGMAudioSource.clip == AudioClip && BGMAudioSource.isPlaying == true && ForceReplay == false)
        {
            return;
        }

        IsBGMNeedToPlay = true;

        if (PlayNow == true)
        {
            BGMAudioSource.clip = AudioClip;
            BGMAudioSource.Play();
            LastBGM = AudioClip;

            foreach (BGMAudioData BGMData in BGMDataList)
            {
                if (BGMData.AudioClip == AudioClip)
                {
                    OnBGMChanged.Invoke(BGMData.BGMType);
                    break;
                }
            }
        }
        else
        {
            BGMQueue.Add(AudioClip);
        }
    }

    public void StopBGM(bool RemoveAllQueue)
    {
        if (RemoveAllQueue == true)
        {
            BGMQueue.Clear();
        }

        IsBGMNeedToPlay = false;
        BGMAudioSource.Stop();
    }

    IEnumerator WaitNextBGMLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (BGMAudioSource.isPlaying == true)
            {
                continue;
            }

            if(IsBGMNeedToPlay == false)
            {
                continue;
            }

            if (BGMQueue.Count > 0)
            {
                PlayBGM(BGMQueue[BGMQueue.Count - 1], true, false);
                BGMQueue.RemoveAt(BGMQueue.Count - 1);
            }
            else
            {
                PlayBGM(LastBGM, true, false);
            }
        }
    }

    public bool IsBGMPlaying(EBGM BGMIndex)
    {
        if (BGMAudioSource.clip == BGMDataList[(int)BGMIndex].AudioClip)
        {
            return true;
        }

        return false;
    }

    public void PlaySFX(ESFX SFXType)
    {
        SFXAudioSource.PlayOneShot(SFXDataList[(int)SFXType].AudioClip);

        if(SFXType == ESFX.SFX_START)
        {
            EventManager.Instance.OnSFXPlayedEvent?.Invoke(SFXType);
        }
    }

    private IEnumerator ReadyToStart_Internal()
    {
        float SFXAudioLength = 0f;
        foreach (SFXAudioData SFXAudioData in SFXDataList)
        {
            if (SFXAudioData.SFXType == ESFX.SFX_START)
            {
                SFXAudioLength = SFXAudioData.AudioClip.length;
            }
        }

        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            if (BGMAudioSource.time + SFXAudioLength + StartSFXOffset >= BGMAudioSource.clip.length)
            {
                PlaySFX(ESFX.SFX_START);
                break;
            }
        }
    }

    public void OnBGMVolumeChanged(Slider BGMSlider)
    {
        SoundSettingData.BGMVolume = BGMAudioSource.volume = BGMSlider.value;
    }

    public void OnSFXVolumeChanged(Slider SFXSlider)
    {
        SoundSettingData.SFXVolume = SFXAudioSource.volume = SFXSlider.value;
    }
}
