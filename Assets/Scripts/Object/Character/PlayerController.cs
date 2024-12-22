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
    Scorpion,
    Spider,
    Tortoise,
}

public class PlayerController : MonoBehaviour
{
    private static PlayerController Instance;

    static public PlayerController Get()
    {
        return Instance;
    }

    void Awake()
    {
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
    private bool IsMoveEnabled = true;
    private Vector2 StartPosition = Vector2.zero;

    //Event
    public UnityEvent<float> OnPlayerAcceleratedEvent;
    public UnityEvent<bool> OnPlayerMovementEnableChangedEvent;
    public UnityEvent OnGameStartEvent;
    public UnityEvent OnGameOverEvent;
    public UnityEvent OnAnimalTryingToChangeEvent;
    public UnityEvent<AnimalType> OnAnimalTypeChangedEvent;

    void Start()
    {
        InitializeInput();
        ChangeAnimal(CurrentAnimalType);

        IsMoveEnabled = false;
        StartPosition = transform.position;
    }

    void Update()
    {
        Update_PlayerMove();
        Update_CheckVelocity();
    }

    public void OnGameStart()
    {
        SetMoveEnabled(true);

        OnGameStartEvent.Invoke();
    }

    public void OnGameOver()
    {
        ResetPlayer();
        SetMoveEnabled(false);

        OnGameOverEvent.Invoke();
    }

    private void ResetPlayer()
    {
        CurrentVelocity = 0f;
        Rigidbody2D PlayerRigidbody = GetComponent<Rigidbody2D>();
        if (PlayerRigidbody != null)
        {
            PlayerRigidbody.velocity = Vector2.zero;
        }

        if (MoveSpeedObject != null)
        {
            MoveSpeedObject.velocity = Vector2.zero;
        }

        transform.position = StartPosition;
    }

    private void InitializeInput()
    {
        CurrentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public InputType GetCurrentAnimalInputType()
    {
        return CurrentAnimalData.InputType;
    }

    public AnimalType GetCurrentAnimalType()
    {
        return CurrentAnimalData.AnimalType;
    }

    private void SetMoveEnabled(bool Enabled)
    {
        IsMoveEnabled = Enabled;
    }

    private void ChangeAnimatorController(RuntimeAnimatorController NewAnimatorController)
    {
        Animator CurrentAnimator = GetComponent<Animator>();
        if (CurrentAnimator != null && gameObject.activeInHierarchy == true)
        {
            CurrentAnimator.SetBool("IsRunning", false);
            CurrentAnimator.runtimeAnimatorController = NewAnimatorController;
        }
    }

    public void ChangeAnimal(AnimalType NewAnimalType)
    {
        CurrentAnimalType = NewAnimalType;
        CurrentAnimalData = AnimalDataManager.Get().GetAnimalData(CurrentAnimalType);
        ChangeAnimatorController(CurrentAnimalData.Animator);
        CurrentInputStackIndex = 0;

        OnAnimalTypeChangedEvent.Invoke(CurrentAnimalType);
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
            case InputType.SpaceBarToCrash:
                {

                    break;
                }
            case InputType.ArrowRightLeft_TakeTurn:
                {
                    //왼쪽
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        CurrentInputStackIndex++;
                        MovePlayer();
                    }
                    //오른쪽
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
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

        // 동작을 진행했을 때 마다
        if (PrevInputStackIndex != CurrentInputStackIndex)
        {
            MovePlayer();
        }

        if (CurrentInputStackIndex >= AnimalDataManager.Get().GetInputStackCount(CurrentAnimalData.InputType))
        {
            CurrentInputStackIndex = 0;
        }
    }

    private void MovePlayer()
    {
        Vector2 MoveForce = new Vector2(AnimalDataManager.Get().GetVelocity(CurrentAnimalData.InputType), 0);
        MoveSpeedObject.AddForce(MoveForce);
        OnPlayerAcceleratedEvent?.Invoke(MoveForce.x);
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
        if (CurrentAnimator == null)
        {
            return;
        }

        bool IsRunning = CurrentVelocity > 0.01f;
        if (IsRunning == true)
        {
            float NewAnimationSpeed = Mathf.Clamp(CurrentVelocity * RunAnimationSpeedRate, RunAnimationMinSpeedRate, RunAnimationMaxSpeedRate);
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

    public void SetIsAnimalChanging(bool IsChanging)
    {
        if (IsMoveEnabled == !IsChanging)
        {
            return;
        }
        IsMoveEnabled = !IsChanging;

        EnableMovement(IsMoveEnabled);
        OnPlayerMovementEnableChangedEvent?.Invoke(IsMoveEnabled);
    }

    public void EnableMovement(bool Enabled)
    {
        if (Enabled == false)
        {
            CurrentVelocity = 0;
            if (MoveSpeedObject != null)
            {
                MoveSpeedObject.velocity = Vector2.zero;
            }
        }
    }
}
