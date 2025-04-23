using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum AnimalType
{
    Bat,
    Bear,
    Bee,
    Boar,
    Crab,
    Dog,
    Duck,
    Frog,
    Pig,
    Rat,
    Scorpion,
    Spider,
    Tortoise,
}

public class PlayerController : GameObjectController, FeverInterface
{
    private static PlayerController Instance;

    static public PlayerController Get()
    {
        return Instance;
    }

    new void Awake()
    {
        InteractCollisionComponent.OnTriggerEnter.AddListener(Interaction);

        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 다른 인스턴스 가 이미 생성된 경우 삭제
            Destroy(gameObject);
            return;
        }
    }

    //Data
    public AnimalType CurrentAnimalType;
    private AnimalData CurrentAnimalData;
    private int CurrentInputStackIndex = 0;
    private Vector2 CurrentMousePosition = Vector2.zero;
    private Vector2 CurrentMouseScrollDelta = Vector2.zero;

    [Header("Animation")]
    public float RunAnimationSpeedRate = 1f;
    public float RunAnimationMaxSpeedRate = 5f;
    public float RunAnimationMinSpeedRate = 2f;

    // Data
    [Header("Movement")]
    public float CurrentVelocity;
    public Rigidbody2D MoveSpeedObject;
    private Vector2 StartPosition = Vector2.zero;

    [Header("Fever")]
    public float FeverMaxSizeScale = 3f;

    public TriggerCollisionComponent InteractCollisionComponent;

    new void Start()
    {
        base.Start();
        InitializeGameObject();
    }

    public override void InitializeGameObject()
    {
        base.InitializeGameObject();

        StartPosition = transform.position;
        CurrentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        UpdateAnimator();
        EventManager.Instance.OnAnimalChangedEvent?.Invoke(true, CurrentAnimalType);
    }

    void Update()
    {
        Update_PlayerMove();
        Update_CheckVelocity();
    }

    protected override void OnAnimalTryingToChange()
    {
        EnableMovement(false);
    }

    public void ChangeAnimalOnPlay(AnimalType NewAnimalType)
    {
        CurrentAnimalType = NewAnimalType;
        UpdateAnimator();

        EventManager.Instance.OnAnimalChangedEvent?.Invoke(false, CurrentAnimalType);
        EnableMovement(true);
    }

    private void UpdateAnimator()
    {
        CurrentAnimalData = AnimalDataManager.Instance.GetAnimalData(CurrentAnimalType);
        ChangeAnimatorController(CurrentAnimalData.Animator);
        CurrentInputStackIndex = 0;
    }

    protected override void ResetGameObject()
    {
        CurrentVelocity = 0f;
        Rigidbody2D PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerRigidbody.velocity = Vector2.zero;

        MoveSpeedObject.velocity = Vector2.zero;

        transform.position = StartPosition;
    }

    protected override void EnableMovement(bool Enabled)
    {
        IsMoveEnabled = Enabled;
        if (Enabled == false)
        {
            CurrentVelocity = 0;
            MoveSpeedObject.velocity = Vector2.zero;
        }

        Animator CurrentAnimator = GetComponent<Animator>();
        CurrentAnimator.SetBool("IsRunning", Enabled);
    }

    public InputType GetCurrentAnimalInputType()
    {
        return CurrentAnimalData.InputType;
    }

    public AnimalType GetCurrentAnimalType()
    {
        return CurrentAnimalData.AnimalType;
    }

    private void ChangeAnimatorController(RuntimeAnimatorController NewAnimatorController)
    {
        Animator CurrentAnimator = GetComponent<Animator>();
        if (gameObject.activeInHierarchy == true)
        {
            CurrentAnimator.SetBool("IsRunning", false);
            CurrentAnimator.runtimeAnimatorController = NewAnimatorController;
        }
    }

    private void Update_PlayerMove()
    {
        if (IsMoveEnabled == false)
        {
            return;
        }

        if (CurrentAnimalData == null)
        {
            return;
        }

        float PrevInputStackIndex = CurrentInputStackIndex;
        switch (CurrentAnimalData.InputType)
        {
            case InputType.AD_TakeTurn:
                {
                    if (Input.GetKeyDown(KeyCode.A) && CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    if (Input.GetKeyDown(KeyCode.D) && CurrentInputStackIndex == 1)
                    {
                        CurrentInputStackIndex++;
                    }

                    break;
                }
            case InputType.MouseScrollDown:
                {
                    Vector2 PrevMouseScrollDelta = CurrentMouseScrollDelta;
                    CurrentMouseScrollDelta = Input.mouseScrollDelta;
                    if (PrevMouseScrollDelta.y > CurrentMouseScrollDelta.y)
                    {
                        CurrentInputStackIndex++;
                    }
                    break;
                }
            case InputType.MouseScrollUp:
                {
                    Vector2 PrevMouseScrollDelta = CurrentMouseScrollDelta;
                    CurrentMouseScrollDelta = Input.mouseScrollDelta;
                    if (PrevMouseScrollDelta.y < CurrentMouseScrollDelta.y)
                    {
                        CurrentInputStackIndex++;
                    }
                    break;
                }
            case InputType.QWER_TakeTurn:
                {
                    if (Input.GetKeyDown(KeyCode.Q) && CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    else if (Input.GetKeyDown(KeyCode.W) && CurrentInputStackIndex == 1)
                    {
                        CurrentInputStackIndex++;
                    }
                    else if (Input.GetKeyDown(KeyCode.E) && CurrentInputStackIndex == 2)
                    {
                        CurrentInputStackIndex++;
                    }
                    else if (Input.GetKeyDown(KeyCode.R) && CurrentInputStackIndex == 3)
                    {
                        CurrentInputStackIndex++;
                    }

                    break;
                }
            case InputType.SpaceBar:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        CurrentInputStackIndex++;
                    }
                    break;
                }
            case InputType.SpaceBarRepeatEnter:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        CurrentInputStackIndex++;
                    }
                    break;
                }
            case InputType.MouseVerticalHorizonal:
                {
                    Vector2 PastMousePosition = CurrentMousePosition;
                    CurrentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    // 왼쪽
                    if (PastMousePosition.x > CurrentMousePosition.x && CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    // 오른쪽
                    else if (PastMousePosition.x < CurrentMousePosition.x && CurrentInputStackIndex == 1)
                    {
                        CurrentInputStackIndex++;
                    }
                    // 위
                    else if (PastMousePosition.y < CurrentMousePosition.y && CurrentInputStackIndex == 2)
                    {
                        CurrentInputStackIndex++;
                    }
                    // 아래
                    else if (PastMousePosition.y > CurrentMousePosition.y && CurrentInputStackIndex == 3)
                    {
                        CurrentInputStackIndex++;
                    }

                    break;
                }
            case InputType.Mouse8:
                {
                    Vector2 PastMousePosition = CurrentMousePosition;
                    CurrentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                    if (CurrentInputStackIndex == 0)
                    {
                        // 왼쪽 위
                        if (PastMousePosition.x > CurrentMousePosition.x &&
                           PastMousePosition.y < CurrentMousePosition.y)
                        {
                            CurrentInputStackIndex++;
                        }
                    }
                    else if (CurrentInputStackIndex == 1)
                    {
                        // 오른쪽 위
                        if (PastMousePosition.x < CurrentMousePosition.x &&
                           PastMousePosition.y < CurrentMousePosition.y)
                        {
                            CurrentInputStackIndex++;
                        }
                    }
                    else if (CurrentInputStackIndex == 2)
                    {
                        // 왼쪽 아래
                        if (PastMousePosition.x > CurrentMousePosition.x &&
                           PastMousePosition.y > CurrentMousePosition.y)
                        {
                            CurrentInputStackIndex++;
                        }
                    }
                    else if (CurrentInputStackIndex == 3)
                    {
                        // 오른쪽 아래
                        if (PastMousePosition.x < CurrentMousePosition.x &&
                           PastMousePosition.y > CurrentMousePosition.y)
                        {
                            CurrentInputStackIndex++;
                        }
                    }
                    else if (CurrentInputStackIndex == 4)
                    {
                        // 오른쪽 위
                        if (PastMousePosition.x < CurrentMousePosition.x &&
                           PastMousePosition.y < CurrentMousePosition.y)
                        {
                            CurrentInputStackIndex++;
                        }
                    }
                    else if (CurrentInputStackIndex == 5)
                    {
                        // 왼쪽 위
                        if (PastMousePosition.x > CurrentMousePosition.x &&
                           PastMousePosition.y < CurrentMousePosition.y)
                        {
                            CurrentInputStackIndex++;
                        }
                    }

                    break;
                }
            case InputType.ArrowRightLeft_TakeTurn:
                {
                    //왼쪽
                    if (Input.GetKeyDown(KeyCode.LeftArrow) &&
                        CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    //오른쪽
                    else if (Input.GetKeyDown(KeyCode.RightArrow) &&
                        CurrentInputStackIndex == 1)
                    {
                        CurrentInputStackIndex++;
                    }

                    break;
                }
            case InputType.QWERASDF:
                {
                    if (Input.GetKeyDown(KeyCode.Q) ||
                       Input.GetKeyDown(KeyCode.W) ||
                       Input.GetKeyDown(KeyCode.E) ||
                       Input.GetKeyDown(KeyCode.R) ||
                       Input.GetKeyDown(KeyCode.A) ||
                       Input.GetKeyDown(KeyCode.S) ||
                       Input.GetKeyDown(KeyCode.D) ||
                       Input.GetKeyDown(KeyCode.F))
                    {
                        CurrentInputStackIndex++;
                    }
                }
                break;

            case InputType.ZXDotSlash:
                {
                    if (Input.GetKeyDown(KeyCode.Z) ||
                        Input.GetKeyDown(KeyCode.X) ||
                        Input.GetKeyDown(KeyCode.Period) ||
                        Input.GetKeyDown(KeyCode.Comma))
                    {
                        CurrentInputStackIndex++;
                    }
                }
                break;
            case InputType.ArrowAll:
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow) ||
                       Input.GetKeyDown(KeyCode.LeftArrow) ||
                       Input.GetKeyDown(KeyCode.LeftArrow) ||
                       Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        CurrentInputStackIndex++;
                    }
                }
                break;
            case InputType.QWAS_IOKL_TakeTurn:
                {
                    if (Input.GetKey(KeyCode.Q) &&
                       Input.GetKey(KeyCode.W) &&
                       Input.GetKey(KeyCode.A) &&
                       Input.GetKey(KeyCode.S) &&
                       CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    else if (Input.GetKey(KeyCode.I) &&
                            Input.GetKey(KeyCode.O) &&
                            Input.GetKey(KeyCode.K) &&
                            Input.GetKey(KeyCode.L) &&
                            CurrentInputStackIndex == 1)
                    {
                        CurrentInputStackIndex++;
                    }
                }
                break;
            case InputType.MouseLeftRight_TakeTurn:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0) && CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    else if (Input.GetKeyDown(KeyCode.Mouse1) && CurrentInputStackIndex == 1)
                    {
                        CurrentInputStackIndex++;
                    }
                }
                break;
        }

        int InputStackCount = AnimalDataManager.Instance.GetInputStackCount(CurrentAnimalData.InputType);

        // Input Stack 이 Custom 일 때
        if (InputStackCount == 0)
        {
            if(CurrentAnimalData.InputType == InputType.SpaceBarRepeatEnter)
            {
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    // 최소 3개 이상 차지 하지 않으면 동작하지 않음
                    if (CurrentInputStackIndex < 3)
                    {
                        CurrentInputStackIndex = 0;
                        return;
                    }

                    MovePlayer(CurrentInputStackIndex);
                    CurrentInputStackIndex = 0;
                }
            }
        }
        else
        {
            // 동작을 진행했을 때 마다
            if (PrevInputStackIndex != CurrentInputStackIndex)
            {
                MovePlayer();
            }

            if (CurrentInputStackIndex >= InputStackCount)
            {
                CurrentInputStackIndex = 0;
            }
        }
    }

    private void MovePlayer()
    {
        MovePlayer(1f);
    }

    private void MovePlayer(float VelocityRate)
    {
        Vector2 MoveForce = new Vector2(AnimalDataManager.Instance.GetVelocity(CurrentAnimalData.InputType), 0) * VelocityRate;

        // 피버타임일 경우 반대로
        if (IsFever == true)
        {
            MoveForce *= -1;
        }

        MoveSpeedObject.AddForce(MoveForce);
        EventManager.Instance.OnPlayerAcceleratedEvent?.Invoke(MoveForce.x);
    }

    private void Update_CheckVelocity()
    {
        if (MoveSpeedObject == null)
        {
            return;
        }

        CurrentVelocity = MoveSpeedObject.velocity.x;

        if (IsMoveEnabled == false)
        {
            CurrentVelocity = 0;
        }

        Animator CurrentAnimator = GetComponent<Animator>();

        bool IsRunning = (IsMoveEnabled == true) && (Mathf.Abs(CurrentVelocity) > 0.01f);
        if (IsRunning == true)
        {
            float NewAnimationSpeed = Mathf.Clamp(Mathf.Abs(CurrentVelocity) * RunAnimationSpeedRate, RunAnimationMinSpeedRate, RunAnimationMaxSpeedRate);
            CurrentAnimator.speed = NewAnimationSpeed;
        }
        else
        {
            CurrentAnimator.speed = 0.5f;
        }

        CurrentAnimator.SetBool("IsRunning", IsRunning);
    }

    public float GetVelocity()
    {
        return CurrentVelocity;
    }

    private void Interaction(GameObject InteractObject)
    {
        InteractableInterface InteractableObject = InteractObject.GetComponent<InteractableInterface>();
        if(InteractableObject != null)
        {
            InteractableObject.Interaction(gameObject);
        }
    }

    // FeverInterface
    protected override IEnumerator FeverReadyForStart_Internal(float FirstDelay, float GrowDuration, float DelayAfterGrown, float TurnDuration, string EmojiKey, float EmojiDuration, float LastDuration)
    {
        yield return new WaitForSeconds(FirstDelay);

        CustomAnimationComponent MovementAnimationComponent = GetComponentInChildren<CustomAnimationComponent>();

        // 플레이어 크기 확대
        MovementAnimationComponent.GrowCharacter(true, GrowDuration);
        SoundManager SoundManager = SoundManager.Instance;
        SoundManager.PlaySFX(SoundManager.ESFX.SFX_GROW);
        yield return new WaitForSeconds(GrowDuration + DelayAfterGrown);

        /// Fever Time 용 애니메이션 재생
        Animator CurrentAnimator = GetComponent<Animator>();
        if (gameObject.activeInHierarchy == true)
        {
            CurrentAnimator.SetBool("IsFeverTime", true);
        }

        // 뒤돌기
        MovementAnimationComponent.TurnReverse(true);
        yield return new WaitForSeconds(TurnDuration);

        // 피버 이모지
        EmojiComponent EmojiComponent = GetComponentInChildren<EmojiComponent>();
        EmojiComponent.PlayEmojiAnimation(EmojiKey);

        yield return new WaitForSeconds(EmojiDuration + LastDuration);
    }

    protected override IEnumerator FeverReadyForFinish_Internal(float FinishFirstDelay, float ShrinkDuration, float DelayAfterShrink, string FinishEmojiKey, float FinishEmojiDuration, float FinishLastDuration)
    {
        MoveToDefaultPos(FinishFirstDelay);
        yield return new WaitForSeconds(FinishFirstDelay);

        /// 기본 애니메이션 재생 
        Animator CurrentAnimator = GetComponent<Animator>();
        if (gameObject.activeInHierarchy == true)
        {
            CurrentAnimator.SetBool("IsFeverTime", false);
        }

        CustomAnimationComponent MovementAnimationComponent = GetComponentInChildren<CustomAnimationComponent>();

        // 플레이어 크기 축소
        MovementAnimationComponent.GrowCharacter(false, ShrinkDuration);

        SoundManager SoundManager = SoundManager.Instance;
        SoundManager.PlaySFX(SoundManager.ESFX.SFX_SHRINK);
        yield return new WaitForSeconds(DelayAfterShrink);

        // 이모지
        EmojiComponent EmojiComponent = GetComponentInChildren<EmojiComponent>();
        EmojiComponent.PlayEmojiAnimation(FinishEmojiKey);

        // 점프
        MovementAnimationComponent.PlayJumpEffect();
        yield return new WaitForSeconds(FinishEmojiDuration);

        // 방향 전환
        MovementAnimationComponent.TurnReverse(false);
        yield return new WaitForSeconds(FinishLastDuration);
    }
    // ~ FeverInterface
}
