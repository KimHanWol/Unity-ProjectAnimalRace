using System.Collections;
using UnityEngine;

public class AnimalChanger : SpawnableObject, InteractableInterface
{
    private AnimalDataManager AnimalDataManager;
    protected Animator Animator;
    private BoxCollider2D BoxCollider2D;

    //Data
    protected AnimalData CurrentAnimalData;
    private bool bIsChanged = false;
    public Vector2 OverlapBoundary;
    public float MoveCenterSpeed;
    public string EmojiKey;
    public float JumpDuration;
    private GameObject CurrentInteractObject;
    private bool IsNewAnimal = false;

    //동물이 가운데 위치로 돌아가는 데 걸리는 시간
    public float DefaultMoveDuration = 2f;

    [Header("NewAnimal")]
    public float NewAnimalProbability;

    new void Awake()
    {
        base.Awake();

        EventManager.Instance.OnNewAnimalUnlockFinishedEvent.AddListener(OnNewAnimalUnlockFinished);
    }

    void Start()
    {
        AnimalDataManager = AnimalDataManager.Instance;
        BoxCollider2D = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>();

        InitializeAnimalChanger();
    }

    private void InitializeAnimalChanger()
    {
        float RandomValue = Random.Range(0f, 1f);
        IsNewAnimal = RandomValue < NewAnimalProbability;

        // 언락할 수 있는 동물이 없으면 기존 것
        if (AnimalDataManager.Instance.UnlockedAnimalList.Count < AnimalDataManager.Instance.AnimalDataList.Length)
        {
            IsNewAnimal = false;
        }

        // 스폰 가능한 동물이 하나 밖에 없으면 새로운 것
        if (AnimalDataManager.Instance.UnlockedAnimalList.Count <= 1)
        {
            IsNewAnimal = true;
        }

        if (IsNewAnimal == true)
        {
            CurrentAnimalData = AnimalDataManager.Instance.GetLockedAnimalDataByRandom();
        }
        else
        {
            CurrentAnimalData = AnimalDataManager.GetUnlockedAnimalDataByRandom(PlayerController.Get().GetCurrentAnimalType());
        }
        Animator.runtimeAnimatorController = CurrentAnimalData.Animator;
    }

    //InteractableInterface
    public override void Interaction(GameObject InteractObject)
    {
        if (IsActivated == true)
        {
            return;
        }

        CurrentInteractObject = InteractObject;

        if (IsNewAnimal == true)
        {
            EventManager.Instance.OnNewAnimalUnlockStartEvent?.Invoke(CurrentAnimalData.AnimalType);
        }
        else
        {
            SwitchAnimal(InteractObject);
        }

        base.Interaction(InteractObject);
    }
    //~InteractableInterface

    protected void SwitchAnimal(GameObject InteractObject)
    {
        PlayerController OverlappedPlayer = InteractObject.GetComponent<PlayerController>();
        if (OverlappedPlayer != null)
        {
            EventManager.Instance.OnAnimalTryingToChangeEvent?.Invoke();
            StartCoroutine(SwitchAnimal_Internal(InteractObject.GetComponent<PlayerController>()));
            bIsChanged = true;

            EmojiComponent PlayerEmojiComponent = OverlappedPlayer.GetComponentInChildren<EmojiComponent>();
            PlayerEmojiComponent.PlayEmojiAnimation(EmojiKey);

            HunterController Hunter = GameManager.Instance.Hunter;
            EmojiComponent HunterEmojiComponent = Hunter.GetComponentInChildren<EmojiComponent>();
            HunterEmojiComponent.PlayEmojiAnimation(EmojiKey);
        }
    }

    IEnumerator SwitchAnimal_Internal(PlayerController ColliderPlayer)
    {
        MoveToPlayerComponent MoveToPlayerComponent = GetComponentInChildren<MoveToPlayerComponent>();
        MoveToPlayerComponent.EnableMovement(false);

        CustomAnimationComponent PlayerMoveAnimComponent = ColliderPlayer.GetComponentInChildren<CustomAnimationComponent>();
        PlayerMoveAnimComponent.PlayJumpEffect();

        CustomAnimationComponent MoveAnimComponent = GetComponentInChildren<CustomAnimationComponent>();
        MoveAnimComponent.PlayJumpEffect();

        HunterController Hunter = GameManager.Instance.Hunter;
        CustomAnimationComponent HunterMoveAnimComponent = Hunter.GetComponentInChildren<CustomAnimationComponent>();
        HunterMoveAnimComponent.PlayJumpEffect();

        yield return new WaitForSeconds(JumpDuration);
        MoveToPlayerComponent.EnableMovement(true);

        MoveAnimal(ColliderPlayer);
        ColliderPlayer.ChangeAnimalOnPlay(CurrentAnimalData.AnimalType);
    }

    private void MoveAnimal(PlayerController ColliderPlayer)
    {
        //switch position with animal changer
        Vector2 PlayerPosition = ColliderPlayer.transform.position;
        ColliderPlayer.transform.position = transform.position;
        transform.position = PlayerPosition;

        AnimalType PlayerAnimalType = ColliderPlayer.CurrentAnimalType;
        AnimalData PlayerAnimalData = AnimalDataManager.Instance.GetAnimalData(PlayerAnimalType);
        Animator.runtimeAnimatorController = PlayerAnimalData.Animator;

        StartCoroutine(DisableAnimalChanger());

        ColliderPlayer.MoveToDefaultPos(DefaultMoveDuration);

        SelfDestroy();
    }

    IEnumerator DisableAnimalChanger()
    {
        SpriteRenderer Renderder = GetComponent<SpriteRenderer>();
        if (Renderder == null)
        {
            yield return null;
        }

        Color OriginColor = Renderder.material.color;
        if (Renderder == null)
        {
            yield return null;
        }

        if (BoxCollider2D == null)
        {
            yield return null;
        }

        BoxCollider2D.enabled = false;

        float FadingTime = 0f;
        float Duration = 1.0f;
        while (FadingTime <= Duration)
        {
            yield return new WaitForFixedUpdate();
            FadingTime += Time.deltaTime;

            Color NewColor = new Color(OriginColor.r, OriginColor.g, OriginColor.b, 1 - FadingTime / Duration);
            Renderder.material.color = NewColor;
        }
    }

    private void OnNewAnimalUnlockFinished()
    {
        SwitchAnimal(CurrentInteractObject);
    }
}
