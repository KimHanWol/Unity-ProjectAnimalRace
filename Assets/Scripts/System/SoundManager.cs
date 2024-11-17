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

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public enum EBGM
    {
        BGM_START,
        BGM_PLAYING,
        BGM_GAMEOVER,
    }

    public enum ESFX
    {
        SFX_START,
        SFX_BUTTON,
    }

    [SerializeField] BGMAudioData[] BGMDataList;
    [SerializeField] SFXAudioData[] SFXDataList;

    public AudioSource BGMAudioSource;
    public AudioSource SFXAudioSource;

    private AudioClip LastBGM;
    private List<AudioClip> BGMQueue;

    public UnityEvent<EBGM> OnBGMChanged;
    public UnityEvent<ESFX> OnSFXChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        BGMQueue = new List<AudioClip>();

        StartCoroutine(WaitNextBGMLoop());
    }

    public void PlayBGM(EBGM BGMIndex, bool PlayNow)
    {
        PlayBGM(BGMDataList[(int)BGMIndex].AudioClip, PlayNow);

        //Ready to play
        if(BGMIndex == EBGM.BGM_PLAYING)
        {
            StartCoroutine(ReadyToStart());
        }
    }

    public void PlayBGM(AudioClip AudioClip, bool PlayNow)
    {
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

    IEnumerator WaitNextBGMLoop()
    {
        while(true)
        {
            if (BGMAudioSource.isPlaying == true)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (BGMQueue.Count > 0)
            {
                PlayBGM(BGMQueue[BGMQueue.Count - 1], true);
                BGMQueue.RemoveAt(BGMQueue.Count - 1);
            }
            else
            {
                PlayBGM(LastBGM, true);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StopBGM()
    {
        BGMAudioSource.Stop();
    }

    public bool IsBGMPlaying(EBGM BGMIndex)
    {
        if(BGMAudioSource.clip == BGMDataList[(int)BGMIndex].AudioClip)
        {
            return true;
        }

        return false;
    }

    public void PlaySFX(ESFX SFXIndex)
    {
        SFXAudioSource.PlayOneShot(SFXDataList[(int)SFXIndex].AudioClip);
    }

    IEnumerator ReadyToStart()
    {
        float SFXAudioLength = 0f;
        foreach(SFXAudioData SFXAudioData in SFXDataList)
        {
            if(SFXAudioData.SFXType == ESFX.SFX_START)
            {
                SFXAudioLength = SFXAudioData.AudioClip.length;
            }
        }

        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            if(BGMAudioSource.time + SFXAudioLength >= BGMAudioSource.clip.length)
            {
                PlaySFX(ESFX.SFX_START);
                break;
            }
        }
    }

    public void OnBGMVolumeChanged(Slider BGMSlider)
    {
        BGMAudioSource.volume = BGMSlider.value;
    }

    public void OnSFXVolumeChanged(Slider SFXSlider)
    {
        SFXAudioSource.volume = SFXSlider.value;
    }
}
