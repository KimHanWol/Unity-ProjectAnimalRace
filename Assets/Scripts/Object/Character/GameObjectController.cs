using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameObjectController : MonoBehaviour, FeverInterface
{
    protected bool IsMoveEnabled = false;

    public Vector3 DefaultPosition;

    protected bool IsFever = false;

    protected void Start()
    {
        DefaultPosition = transform.position;

        EventManager EventManager = EventManager.Instance;
        EventManager.OnPlayGameEvent.AddListener(OnPlayGame);
        EventManager.OnGameOverEvent.AddListener(OnGameOver);
        EventManager.OnAnimalTryingToChangeEvent.AddListener(OnAnimalTryingToChange);
    }

    protected virtual void OnPlayGame() { }
    protected virtual void OnGameOver() { }
    protected virtual void OnAnimalTryingToChange() { }

    protected virtual void ResetGameObject() { }
    protected virtual void EnableMovement(bool Enabled) { }

    public void MoveToDefaultPos(float Duration)
    {
        StartCoroutine(MoveToDefaultPos_Internal(Duration));
    }

    private IEnumerator MoveToDefaultPos_Internal(float Duration)
    {
        float LastPositionX = transform.position.x;
        float Distance = DefaultPosition.x - LastPositionX;

        float CurrentTime = 0f;
        // Move to default
        while (CurrentTime < Duration)
        {
            transform.position += new Vector3(Distance * Time.deltaTime / Duration, 0);
            yield return new WaitForFixedUpdate();
            CurrentTime += Time.deltaTime;
        }

        transform.position = DefaultPosition;
    }

    // FeverInterface
    public virtual void FeverInitialize()
    {
        EnableMovement(false);
    }

    public virtual void FeverReadyForStart(float FirstDelay, float GrowDuration, float DelayAfterGrown, float TurnDuration, string EmojiKey, float EmojiDuration, float LastDuration)
    {
        StartCoroutine(FeverReadyForStart_Internal(FirstDelay, GrowDuration, DelayAfterGrown, TurnDuration, EmojiKey, EmojiDuration, LastDuration));
    }

    protected virtual IEnumerator FeverReadyForStart_Internal(float FirstDelay, float GrowDuration, float DelayAfterGrown, float TurnDuration, string EmojiKey, float EmojiDuration, float LastDuration) 
    { 
        yield return null;
    }

    public virtual void FeverStart()
    {
        IsFever = true;
        EnableMovement(true);
    }

    public virtual void FeverReadyForFinish(float FinishFirstDelay, float ShrinkDuration, float DelayAfterShrink, string FinishEmojiKey, float FinishEmojiDuration, float FinishLastDuration)
    {
        EnableMovement(false);
        StartCoroutine(FeverReadyForFinish_Internal(FinishFirstDelay, ShrinkDuration, DelayAfterShrink, FinishEmojiKey, FinishEmojiDuration, FinishLastDuration));
    }

    protected virtual IEnumerator FeverReadyForFinish_Internal(float FinishFirstDelay, float ShrinkDuration, float DelayAfterShrink, string FinishEmojiKey, float FinishEmojiDuration, float FinishLastDuration)
    {
        yield return null;
    }

    public virtual void FeverFinished() 
    {
        IsFever = false;
        EnableMovement(true);
    }
    // ~ FeverInterface

}
