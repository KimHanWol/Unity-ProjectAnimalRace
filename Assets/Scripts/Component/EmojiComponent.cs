using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public struct EmojiAnimationData
{
    public string EmojiKey;
    public AnimationClip EmojiAnimation;
}

public class EmojiComponent : MonoBehaviour
{
    [SerializeField]
    public List<EmojiAnimationData> EmojiAnimationDataList;

    private Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void PlayEmojiAnimation(string EmojiKey)
    {
        foreach(EmojiAnimationData EmojiData in EmojiAnimationDataList)
        {
            if (EmojiData.EmojiKey == EmojiKey)
            {
                Animator.Play(EmojiData.EmojiAnimation.name, 0, 0f);
                break;
            }
        }
    }
}