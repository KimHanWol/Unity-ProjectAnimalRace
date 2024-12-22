using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EmojiComponent : MonoBehaviour
{
    public AnimationClip EmojiAnimation;

    private Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void StartEmojiAnimation()
    {
        Animator.Play(EmojiAnimation.name, 0, 0f);
    }
}