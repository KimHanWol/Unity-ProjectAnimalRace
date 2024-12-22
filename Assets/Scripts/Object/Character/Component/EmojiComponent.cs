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

        PlayerController PlayerController = GetComponentInParent<PlayerController>();
        if (PlayerController != null)
        {
            PlayerController.OnAnimalTryingToChangeEvent.AddListener(StartEmojiAnimation);
        }

        HunterController HunterController = GetComponentInParent<HunterController>();
        if (HunterController != null)
        {
            HunterController.OnAnimalTryingToChangeEvent.AddListener(StartEmojiAnimation);
        }
    }

    private void StartEmojiAnimation()
    {
        Animator.Play(EmojiAnimation.name, 0, 0f);
    }
}