using System;
using System.IO.Pipes;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public enum AnimalType
{
    Bat,
    Crab,
    Dog,
    Duck,
    Frog,
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

    [Header("Animation")]
    public float RunAnimationSpeedRate = 1f;
    public float RunAnimationMaxSpeedRate = 5f;
    public float RunAnimationMinSpeedRate = 2f;

    // Data
    [Header("Movement")]
    public float CurrentVelocity;
    public Rigidbody2D MoveSpeedObject;
    private bool IsMoveEnabled = true;

    //Event
    public UnityEvent<float> OnPlayerAccelerated;
    public UnityEvent<bool> OnPlayerMovementEnableChanged;

    void Start()
    {
        InitializeInput();
        ChangeAnimal(CurrentAnimalType);
    }

    void Update()
    {
        Update_PlayerMove();
        Update_CheckVelocity();
    }

    private void InitializeInput()
    {
        CurrentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public AnimalType GetCurrentAnimalType()
    {
        return CurrentAnimalData.AnimalType;
    }

    private void ChangeAnimatorController(AnimatorController NewAnimatorController)
    {
        Animator CurrentAnimator = GetComponent<Animator>();
        if(CurrentAnimator != null && gameObject.activeInHierarchy == true)
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
    }

    private void Update_PlayerMove()
    {
        if(IsMoveEnabled == false)
        {
            return;
        }

        if(CurrentAnimalData == null)
        {
            return;
        }

        InputData CurrentInputData = CurrentAnimalData.InputData;
        switch(CurrentInputData.InputType)
        {
            case InputType.AD_TakeTurn:
            {
                if(Input.GetKeyDown(KeyCode.A) && CurrentInputStackIndex == 0)
                {
                    CurrentInputStackIndex++;
                }
                if(Input.GetKeyDown(KeyCode.D) && CurrentInputStackIndex == 1)
                {
                    CurrentInputStackIndex++;
                }
                
                break;
            }
            case InputType.MouseScrollDown:
            {

                break;
            }
            case InputType.MouseScrollUp:
            {

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
                    if(PastMousePosition.x > CurrentMousePosition.x &&
                       PastMousePosition.y < CurrentMousePosition.y)
                    {
                        CurrentInputStackIndex++;
                    }
                }
                else if(CurrentInputStackIndex == 1)
                {
                    // 오른쪽 위
                    if (PastMousePosition.x < CurrentMousePosition.x &&
                       PastMousePosition.y < CurrentMousePosition.y)
                    {
                        CurrentInputStackIndex++;
                    }
                }
                else if(CurrentInputStackIndex == 2)
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
            case InputType.ArrowRightLeft:
            {
                //왼쪽
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    CurrentInputStackIndex++;
                }
                //오른쪽
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    CurrentInputStackIndex++;
                }

                break;
            }
        }

        if (CurrentInputStackIndex >= CurrentAnimalData.InputData.InputStackCount)
        {
            CurrentInputStackIndex = 0;

            //Move Player
            Vector2 MoveForce = new Vector2(CurrentAnimalData.InputData.Veclocity, 0);
            MoveSpeedObject.AddForce(MoveForce);
            OnPlayerAccelerated?.Invoke(MoveForce.x);
        }
    }

    private void Update_CheckVelocity()
    {
        if(MoveSpeedObject == null)
        {
            return;
        }

        CurrentVelocity = MoveSpeedObject.velocity.x;

        if(IsMoveEnabled == false)
        {
            CurrentVelocity = 0;
        }

        Animator CurrentAnimator = GetComponent<Animator>();
        if(CurrentAnimator == null)
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
        OnPlayerMovementEnableChanged?.Invoke(IsMoveEnabled);
    }

    public void EnableMovement(bool Enabled) 
    {
        if (Enabled == false)
        {
            CurrentVelocity = 0;
            if(MoveSpeedObject != null)
            {
                MoveSpeedObject.velocity = Vector2.zero;
            }
        }
    }
}
