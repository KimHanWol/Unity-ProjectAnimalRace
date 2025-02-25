using System.Collections;
using UnityEngine;

public class AnimalChanger : SpawnableObject, InteractableInterface
{
    private AnimalDataManager AnimalDataManager;
    private Animator Animator;
    private BoxCollider2D BoxCollider2D;

    //Data
    private AnimalData CurrentAnimalData;
    private bool bIsChanged = false;
    public Vector2 OverlapBoundary;
    public float MoveCenterSpeed;
    public string EmojiKey;
    public float JumpDuration;

    //동물이 가운데 위치로 돌아가는 데 걸리는 시간
    public float DefaultMoveDuration = 2f;

    //InteractableInterface
    public override void Interaction(GameObject InteractObject)
    {
        PlayerController OverlappedPlayer = InteractObject.GetComponent<PlayerController>();
        if (OverlappedPlayer != null)
        {
            EventManager.Instance.OnAnimalTryingToChangeEvent?.Invoke();
            StartCoroutine(SwitchAnimal(InteractObject.GetComponent<PlayerController>()));
            bIsChanged = true;

            EmojiComponent PlayerEmojiComponent = OverlappedPlayer.GetComponentInChildren<EmojiComponent>();
            PlayerEmojiComponent.PlayEmojiAnimation(EmojiKey);
        }

        base.Interaction(InteractObject);
    }
    //~InteractableInterface

    void Start()
    {
        AnimalDataManager = AnimalDataManager.Instance;
        BoxCollider2D = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>();

        CurrentAnimalData = AnimalDataManager.GetUnlockedAnimalDataByRandom(PlayerController.Get().GetCurrentAnimalType());
        Animator.runtimeAnimatorController = CurrentAnimalData.Animator;
    }

    public override bool IsSpawnable()
    {
        // 기본 캐릭터 밖에 언락이 되지 않았을 때 스폰 불가능
        if (AnimalDataManager.Instance.UnlockedAnimalList.Count <= 1)
        {
            if(GameManager.Instance.Player.CurrentAnimalType == AnimalDataManager.Instance.UnlockedAnimalList[0])
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator SwitchAnimal(PlayerController ColliderPlayer)
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
}
