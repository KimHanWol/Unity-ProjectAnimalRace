using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum AnimalType
{
    Dog,
}

public class PlayerController : MonoBehaviour
{
    //GameObject
    public GameObject Camera;
    //public AnimationController AnimationController;

    //Data
    public AnimalType CurrentAnimalType;
    private AnimalData CurrentAnimalData;
    private int CurrentInputStackIndex = 0;

    void Start()
    {
        ChangeAnimal(CurrentAnimalType);
    }

    void Update()
    {
        Update_CameraPosition();
        Update_PlayerMove();
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

                if(CurrentInputStackIndex > CurrentAnimalData.InputData.InputStackCount)
                {
                    CurrentInputStackIndex = 0;
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
                
                break;
            }
            case InputType.SpaceBar:
            {

                break;
            }
        }
    }
}
