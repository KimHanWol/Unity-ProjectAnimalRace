using System.Collections;
using UnityEngine;

public class AnimalChanger : RuningObject
{
    private AnimalDataManager AnimalDataManager;
    private Animator Animator;
    private BoxCollider2D BoxCollider2D;

    //Data
    private AnimalData CurrentAnimalData;
    private bool bIsChanged = false;
    public Vector2 OverlapBoundary;
    public float MoveCenterSpeed;
    public float ChangeEffectForce = 100f;

    // Start is called before the first frame update
    void Start()
    {
        AnimalDataManager = AnimalDataManager.Get();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>();

        CurrentAnimalData = AnimalDataManager.GetRandomAnimalData(PlayerController.Get().GetCurrentAnimalType());
        Animator.runtimeAnimatorController = CurrentAnimalData.Animator;
    }

    public void InitializeAnimalChanger()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Update_CheckOverlap();
        Update_ObjectMovement();
    }

    private void Update_CheckOverlap()
    {
        if (bIsChanged == true)
        {
            return;
        }

        Vector2 ColliderSize = OverlapBoundary;

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.localPosition, ColliderSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            PlayerController ColliderPlayer = collider.GetComponent<PlayerController>();
            // TODO: AnimalChanger 에서 하지 말고 Player 의 컴포넌트에서
            // 전면의 장애물을 인식해서 동작하도록 하자
            if(ColliderPlayer != null)
            {
                EventManager.Instance.OnAnimalTryingToChangeEvent?.Invoke();
                StartCoroutine(SwitchAnimal(ColliderPlayer));
                bIsChanged = true;
                break;
            }
        }
    }

    IEnumerator SwitchAnimal(PlayerController ColliderPlayer)
    {
        PlayAnimalChangeEffect(ColliderPlayer);
        yield return new WaitForSeconds(1f);
        EndAnnimalChangeEffect(ColliderPlayer);

        StartCoroutine(StartMoveAnimal(ColliderPlayer));
        ColliderPlayer.ChangeAnimalOnPlay(CurrentAnimalData.AnimalType);
    }

    private void PlayAnimalChangeEffect(PlayerController ColliderPlayer)
    {
        IsNeedToStopMove = true;

        Rigidbody2D PlayerRigidbody = ColliderPlayer.GetComponent<Rigidbody2D>();
        PlayerRigidbody.velocity = Vector2.zero;
        PlayerRigidbody.AddForce(new Vector2(0, ChangeEffectForce));

        Rigidbody2D AnimalChangerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        AnimalChangerRigidbody.velocity = Vector2.zero;
        AnimalChangerRigidbody.AddForce(new Vector2(0, ChangeEffectForce));

        HunterController Hunter = GameManager.Instance.Hunter;
        Rigidbody2D HunterRigidbody = Hunter.gameObject.GetComponent<Rigidbody2D>();
        HunterRigidbody.velocity = Vector2.zero;
        HunterRigidbody.AddForce(new Vector2(0, ChangeEffectForce));
    }

    private void EndAnnimalChangeEffect(PlayerController ColliderPlayer)
    {
        IsNeedToStopMove = false;
    }

    IEnumerator StartMoveAnimal(PlayerController ColliderPlayer)
    {
        AnimalType PlayerAnimalType = ColliderPlayer.CurrentAnimalType;

        //switch position
        Vector2 PlayerPosition = ColliderPlayer.transform.position;
        ColliderPlayer.transform.position = transform.position;
        transform.position = PlayerPosition;

        AnimalData PlayerAnimalData = AnimalDataManager.Get().GetAnimalData(PlayerAnimalType);
        Animator.runtimeAnimatorController = PlayerAnimalData.Animator;

        StartCoroutine(DisableAnimalChanger());

        // Move to zero
        while (ColliderPlayer.transform.position.x > 0)
        {
            Vector2 NewPosition = ColliderPlayer.transform.position;
            NewPosition.x -= MoveCenterSpeed * Time.deltaTime;
            ColliderPlayer.transform.position = NewPosition;
            yield return new WaitForFixedUpdate();
        }

        // Set to zero
        Vector2 NewPostion = ColliderPlayer.transform.position;
        NewPostion.x = 0;
        ColliderPlayer.transform.position = NewPostion;

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

    private void SelfDestroy()
    {
        SpawnableObject SpawnableObject = GetComponent<SpawnableObject>();
        SpawnableObject.IsDestroying = true;
    }
}
