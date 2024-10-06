using System;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public enum AnimalType
{
    Dog,
}

public class PlayerController : MonoBehaviour
{
    //GameObject
    public GameObject Camera;

    //Component
    private AnimalDataManager AnimalDataManager;
    private Rigidbody2D Rigidbody2D;

    //Data
    public AnimalType CurrentAnimalType;
    private AnimalData CurrentAnimalData;
    private int CurrentInputStackIndex = 0;
    private Vector2 CurrentMousePosition = Vector2.zero;

    //Event
    public UnityEvent<float> OnPlayerAccelerated;

    void Start()
    {
        AnimalDataManager = GameManager.Get().AnimalDataManger;
        Rigidbody2D = GetComponent<Rigidbody2D>();

        InitializeInput();
        ChangeAnimal(CurrentAnimalType);
    }

    void Update()
    {
        Update_CameraPosition();
        Update_PlayerMove();
        Update_CheckAnimation();
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
        CurrentAnimalData = AnimalDataManager.GetAnimalData(CurrentAnimalType);
        ChangeAnimatorController(CurrentAnimalData.Animator);
        CurrentInputStackIndex = 0;
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
                    CurrentInputStackIndex = 0;
                }

                break;
            }
        }

        if (CurrentInputStackIndex >= CurrentAnimalData.InputData.InputStackCount)
        {
            CurrentInputStackIndex = 0;
            //Move Player
            OnPlayerAccelerated?.Invoke(CurrentAnimalData.InputData.Veclocity);
        }
    }

    private void Update_CheckAnimation()
    {
        Animator CurrentAnimator = GetComponent<Animator>();

        bool IsRunning = Rigidbody2D.velocity.x > 0;
        if (IsRunning == true)
        {
            float NewAnimationSpeed = Mathf.Clamp(Rigidbody2D.velocity.x * GameManager.Get().RunAnimationSpeedRate, 0, GameManager.Get().RunAnimationMaxSpeedRate);
            CurrentAnimator.speed = NewAnimationSpeed;
        }
        else
        {
            CurrentAnimator.speed = 1;
        }

        CurrentAnimator.SetBool("IsRunning", IsRunning);
    }
}
