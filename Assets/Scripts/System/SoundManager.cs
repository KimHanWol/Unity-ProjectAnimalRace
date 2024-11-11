using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioClip[] BGMList;
    [SerializeField] AudioClip[] SFXList;

    public AudioSource BGMAudioSource;
    public AudioSource SFXAudioSource;

    private AudioClip LastBGM;
    private List<AudioClip> BGMQueue;

    public UnityEvent<EBGM> OnBGMChanged;

    public enum EBGM
    {
        BGM_START,
        BGM_PLAYING,
        BGM_GAMEOVER,
    }

    public enum ESFX
    {
        SFX_BUTTON,
    }

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
        PlayBGM(BGMList[(int)BGMIndex], PlayNow);
    }

    public void PlayBGM(AudioClip AudioClip, bool PlayNow)
    {
        if (PlayNow == true)
        {
            BGMAudioSource.clip = AudioClip;
            BGMAudioSource.Play();
            LastBGM = AudioClip;

            int BGMIndex = System.Array.IndexOf(BGMList, AudioClip);
            OnBGMChanged.Invoke((EBGM)BGMIndex);
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
        if(BGMAudioSource.clip == BGMList[(int)BGMIndex])
        {
            return true;
        }

        return false;
    }

    public void PlaySFX(ESFX SFXIndex)
    {
        SFXAudioSource.PlayOneShot(SFXList[(int)SFXIndex]);
    }
}
