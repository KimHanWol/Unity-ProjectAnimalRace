using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class HunterController : GameObjectController
{
    public float HunterMovementRate = 20f;

    [Header("Force")]
    public float ForceInitial;
    public float ForceIncreaseRate;

    [Header("Delay")]
    public float FirstDelay = 1f;
    public float DelayInitial = 1f;
    public float DelayDecreaseRate = 0.01f;
    public float DelayMin = 0.05f;

    public TriggerCollisionComponent CanHitCollisionCompoment;
    public TriggerCollisionComponent GameOverCollisionComponent;

    private float CurrentForceRate = 1f;
    private float CurrentDurationRate = 1f;
    private Vector2 StartPosition = Vector2.zero;
    private float CurrentAnimalVelocity = 0f;

    private Rigidbody2D RigidBody2D;
    private Animator Animator;

    new void Start()
    {
        base.Start();
        RigidBody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        StartPosition = transform.position;
        EventManager.Instance.OnPlayerAcceleratedEvent.AddListener(OnAnimalAccelerated);
        EventManager.Instance.OnAnimalChangedEvent.AddListener(OnAnimalChanged);

        CanHitCollisionCompoment.OnTriggerEnter.AddListener(OnCanHit);
        CanHitCollisionCompoment.OnTriggerExit.AddListener(OnCantHit);

        GameOverCollisionComponent.OnTriggerEnter.AddListener(OnCatchAnimal);
    }

    protected override void OnPlayGame()
    {
        EnableMovement(true);
    }

    protected override void OnGameOver()
    {
        ResetGameObject();
    }

    protected override void OnAnimalTryingToChange()
    {
        IsMoveEnabled = false;
        EnableMovement(IsMoveEnabled);
    }
    
    private void OnAnimalChanged(bool IsInitializing, AnimalType NewAnimalType)
    {
        IsMoveEnabled = true;
        if(IsInitializing == false)
        {
            EnableMovement(IsMoveEnabled);
        }
    }

    protected override void ResetGameObject()
    {
        Rigidbody2D PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerRigidbody.velocity = Vector2.zero;

        CurrentForceRate = 1f;
        CurrentDurationRate = 1f;

        transform.position = StartPosition;

        Animator.SetBool("IsRunning", false);

        StopAllCoroutines();
    }

    protected void OnAnimalAccelerated(float MoveForce)
    {
        CurrentAnimalVelocity = MoveForce;
        RigidBody2D.AddForce(new Vector2(-MoveForce * HunterMovementRate, 0));
    }

    protected override void EnableMovement(bool Enabled)
    {
        IsMoveEnabled = Enabled;
        if(Enabled == true)
        {
            StartCoroutine(WaitFirstDelay());
        }
        else
        {
            Animator.SetBool("IsRunning", false);
        }
    }

    private IEnumerator WaitFirstDelay()
    {
        yield return new WaitForSeconds(FirstDelay);
        StartCoroutine(MoveHunter());
    }

    private IEnumerator MoveHunter()
    {
        float CurrentDuration = 1f;
        Animator.SetBool("IsRunning", true);

        while (IsMoveEnabled == true)
        {
            //피버타임일 때는 거리에 비례해서 빨라짐
            if (IsFever == true)
            {
                PlayerController PlayerController = GameManager.Instance.Player;
                float MaxDistance = Mathf.Abs(DefaultPosition.x - PlayerController.DefaultPosition.x);
                //TODO: GameSetting 같은 데이터로 따로 Offset 정의하기
                float CurrentDistance = Mathf.Clamp(Mathf.Abs(transform.position.x - PlayerController.DefaultPosition.x) - 1.5f, 0.001f, MaxDistance);

                CurrentDuration = Mathf.Clamp(DelayInitial * CurrentDistance / MaxDistance, 0.001f, DelayInitial);
            }
            else
            {
                CurrentDuration = Mathf.Clamp(DelayInitial * CurrentDurationRate, DelayMin, DelayInitial);
            }

            Internal_MoveHunter();
            yield return new WaitForSeconds(CurrentDuration);

            CurrentDurationRate -= DelayDecreaseRate;
            CurrentForceRate += ForceIncreaseRate;
        }
    }

    private void Internal_MoveHunter()
    {
        float CurrentForce = ForceInitial;

        // 피버면 반대로 고정값
        if (IsFever == true)
        {
            CurrentForce *= -1;
        }
        else
        {
            CurrentForce *= CurrentForceRate;
        }

        RigidBody2D.AddForce(new Vector2(CurrentForce, 0));
    }

    private void OnCanHit(GameObject OverlappedGameObject)
    {
        Animator.SetBool("CanHit", true);
    }

    private void OnCantHit(GameObject OverlappedGameObject)
    {
        Animator.SetBool("CanHit", false);
    }

    private void OnCatchAnimal(GameObject OverlappedGameObject)
    {
        EventManager.Instance.OnGameOverEvent?.Invoke();
    }

    // FeverInterface
    public override void FeverInitialize()
    {
        base.FeverInitialize();
    }

    protected override IEnumerator FeverReadyForStart_Internal(float FirstDelay, float GrowDuration, float DelayAfterGrown, float TurnDuration, string EmojiKey, float EmojiDuration, float LastDuration)
    {
        // 원래 위치로 이동
        MoveToDefaultPos(FirstDelay);
        yield return new WaitForSeconds(FirstDelay);

        // 이모지 재생
        EmojiComponent EmojiComponent = GetComponentInChildren<EmojiComponent>();
        EmojiComponent.PlayEmojiAnimation(EmojiKey);

        // 점프 
        CustomAnimationComponent MovementAnimationComponent = GetComponentInChildren<CustomAnimationComponent>();
        MovementAnimationComponent.PlayJumpEffect();
        yield return new WaitForSeconds(GrowDuration + DelayAfterGrown);

        MovementAnimationComponent.PlayShakeEffect(EmojiDuration + TurnDuration + LastDuration);
        yield return new WaitForSeconds(EmojiDuration + TurnDuration);

        MovementAnimationComponent.TurnReverse(true);
        yield return new WaitForSeconds(LastDuration);

        Animator.SetBool("IsRunning", true);
    }

    protected override IEnumerator FeverReadyForFinish_Internal(float FinishFirstDelay, float ShrinkDuration, float DelayAfterShrink, string FinishEmojiKey, float FinishEmojiDuration, float FinishLastDuration)
    {
        yield return new WaitForSeconds(FinishFirstDelay);

        MoveToDefaultPos(ShrinkDuration);
        yield return new WaitForSeconds(ShrinkDuration);

        //뒤돌기
        CustomAnimationComponent MovementAnimationComponent = GetComponentInChildren<CustomAnimationComponent>();
        MovementAnimationComponent.TurnReverse(false);
        yield return new WaitForSeconds(DelayAfterShrink);

        EmojiComponent EmojiComponent = GetComponentInChildren<EmojiComponent>();
        EmojiComponent.PlayEmojiAnimation(FinishEmojiKey);
        yield return new WaitForSeconds(FinishEmojiDuration + FinishLastDuration);
    }
    // ~ FeverInterface
}
