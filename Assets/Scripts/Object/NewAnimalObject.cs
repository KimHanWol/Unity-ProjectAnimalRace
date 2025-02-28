using UnityEngine;

public class NewAnimalObject : AnimalChanger
{
    private GameObject CurrentInteractObject;

    private void Awake()
    {
        EventManager.Instance.OnNewAnimalUnlockFinishedEvent.AddListener(OnNewAnimalUnlockFinished);
    }

    protected override void InitializeAnimalData()
    {
        CurrentAnimalData = AnimalDataManager.Instance.GetLockedAnimalDataByRandom();
        Animator.runtimeAnimatorController = CurrentAnimalData.Animator;
    }

    public override bool IsSpawnable()
    {
        // 이미 언락을 다 했으면 스폰 불가
        if(AnimalDataManager.Instance.AnimalDataList.Length == AnimalDataManager.Instance.UnlockedAnimalList.Count)
        {
            return false;
        }

        return true;
    }

    //InteractableInterface
    public override void Interaction(GameObject InteractObject)
    {
        CurrentInteractObject = InteractObject;
        PlayerController OverlappedPlayer = CurrentInteractObject.GetComponent<PlayerController>();
        if (OverlappedPlayer != null)
        {
            EventManager.Instance.OnNewAnimalUnlockStartEvent?.Invoke(CurrentAnimalData.AnimalType);
        }
    }
    //~InteractableInterface

    private void OnNewAnimalUnlockFinished()
    {
        SwitchAnimal(CurrentInteractObject);
        CurrentInteractObject = null;
    }

    //SwitchAnimal
}
