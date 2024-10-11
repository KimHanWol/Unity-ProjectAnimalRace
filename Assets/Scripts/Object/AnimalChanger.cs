using UnityEngine;

public class AnimalChanger : RuningObject
{
    private AnimalDataManager AnimalDataManager;
    private Animator Animator;

    //Data
    private AnimalData CurrentAnimalData;
    public Vector2 OverlapBoundary;

    // Start is called before the first frame update
    void Start()
    {
        AnimalDataManager = AnimalDataManager.Get();
        CurrentAnimalData = AnimalDataManager.GetRandomAnimalData();
        Animator = GetComponent<Animator>();
        if (Animator != null)
        {
            Animator.runtimeAnimatorController = CurrentAnimalData.Animator;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Update_CheckOverlap();
    }

    private void Update_CheckOverlap()
    {
        if(IsActivated == true)
        {
            return;
        }

        Vector2 ColliderLocation = (Vector2)transform.localPosition + new Vector2(0, -0.5f);
        Vector2 ColliderSize = OverlapBoundary;

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.localPosition, ColliderSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            PlayerController ColliderPlayer = collider.GetComponent<PlayerController>();
            if (ColliderPlayer != null)
            {
                ChangeAnimal(ColliderPlayer);
                IsActivated = true;
                break;
            }
        }
    }

    private void ChangeAnimal(PlayerController ColliderPlayer)
    {
        AnimalType PlayerAnimalType = ColliderPlayer.CurrentAnimalType;
        Vector2 PlayerPosition = ColliderPlayer.transform.position;

        ColliderPlayer.ChangeAnimal(CurrentAnimalData.AnimalType);
        ColliderPlayer.transform.position = transform.position;

        transform.position = PlayerPosition;
        AnimalData PlayerAnimalData = AnimalDataManager.Get().GetAnimalData(PlayerAnimalType);
        if (PlayerAnimalData != null)
        {
            Animator.runtimeAnimatorController = PlayerAnimalData.Animator;
        }
    }
}
