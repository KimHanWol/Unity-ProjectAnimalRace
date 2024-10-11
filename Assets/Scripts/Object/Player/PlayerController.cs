using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public enum AnimalType
{
    Dog,
    Crab,
}

public class PlayerController : MonoBehaviour
{
    //GameObject
    public GameObject Camera;

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

    //Event
    public UnityEvent<float> OnPlayerAccelerated;
    public UnityEvent OnAnimalChanged;

    void Start()
    {
        InitializeInput();
        ChangeAnimal(CurrentAnimalType);
    }

    void Update()
    {
        Update_CameraPosition();
        Update_PlayerMove();
        Update_CheckVelocity();
    }

    private void InitializeInput()
    {
        CurrentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void Update_CameraPosition()
    {
        Vector3 NewPosition = Camera.transform.localPosition;
        NewPosition.x = transform.localPosition.x;
        Camera.transform.localPosition = NewPosition;
    }

    private void ChangeAnimatorController(AnimatorController NewAnimatorController)
    {
        Animator CurrentAnimator = GetComponent<Animator>();
        CurrentAnimator.runtimeAnimatorController = NewAnimatorController;
    }

    public void ChangeAnimal(AnimalType NewAnimalType)
    {
        CurrentAnimalType = NewAnimalType;
        CurrentAnimalData = AnimalDataManager.Get().GetAnimalData(CurrentAnimalType);
        ChangeAnimatorController(CurrentAnimalData.Animator);
        CurrentInputStackIndex = 0;
        OnAnimalChanged?.Invoke();
    }

    private void Update_PlayerMove()
    {
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
                CurrentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                if (CurrentInputStackIndex == 0)
                {
                    // 왼쪽
                    if (PastMousePosition.x > CurrentMousePosition.x && CurrentInputStackIndex == 0)
                    {
                        CurrentInputStackIndex++;
                    }
                    // 오른쪽
                    else if (PastMousePosition.x > CurrentMousePosition.x && CurrentInputStackIndex == 1)
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
            MoveSpeedObject.AddForce(new Vector2(CurrentAnimalData.InputData.Veclocity, 0));
            OnPlayerAccelerated?.Invoke(CurrentAnimalData.InputData.Veclocity);
        }
    }

    private void Update_CheckVelocity()
    {
        if(MoveSpeedObject == null)
        {
            return;
        }

        CurrentVelocity = MoveSpeedObject.velocity.x;
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
}
