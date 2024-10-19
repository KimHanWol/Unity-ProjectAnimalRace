using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.VisualScripting.StickyNote;

public class AnimalChanger : RuningObject
{
    private AnimalDataManager AnimalDataManager;
    private Animator Animator;

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
        CurrentAnimalData = AnimalDataManager.GetRandomAnimalData(PlayerController.Get().GetCurrentAnimalType());
        Animator = GetComponent<Animator>();
        if (Animator != null)
        {
            Animator.runtimeAnimatorController = CurrentAnimalData.Animator;
        }
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
        if(bIsChanged == true)
        {
            return;
        }

        Vector2 ColliderSize = OverlapBoundary;

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.localPosition, ColliderSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            PlayerController ColliderPlayer = collider.GetComponent<PlayerController>();
            if (ColliderPlayer != null)
            {
                StartCoroutine(SwitchAnimal(ColliderPlayer));
                bIsChanged = true;
                break;
            }
        }
    }

    IEnumerator SwitchAnimal(PlayerController ColliderPlayer)
    {
        PlayAnimalChangeEffect(ColliderPlayer);
        yield return new WaitForSeconds(1);
        EndAnnimalChangeEffect(ColliderPlayer);

        StartCoroutine(MoveAnimal(ColliderPlayer));
    }

    private void PlayAnimalChangeEffect(PlayerController ColliderPlayer)
    {
        IsNeedToStopMove = true;

        ColliderPlayer.SetIsAnimalChanging(true);
        Animator CurrentAnimator = GetComponent<Animator>();
        if (CurrentAnimator != null)
        {
            CurrentAnimator.SetBool("IsRunning", false);
        }

        Rigidbody2D PlayerRigidbody = ColliderPlayer.GetComponent<Rigidbody2D>();
        Rigidbody2D AnimalChangerRigidbody = gameObject.GetComponent<Rigidbody2D>();

        if (PlayerRigidbody != null)
        {
            PlayerRigidbody.velocity = Vector2.zero; 
            PlayerRigidbody.AddForce(new Vector2(0, ChangeEffectForce));
        }

        if (AnimalChangerRigidbody != null)
        {
            AnimalChangerRigidbody.velocity = Vector2.zero;
            AnimalChangerRigidbody.AddForce(new Vector2(0, ChangeEffectForce));
        }
    }

    private void EndAnnimalChangeEffect(PlayerController ColliderPlayer)
    {
        IsNeedToStopMove = false;
        ColliderPlayer.SetIsAnimalChanging(false);
    }

    IEnumerator MoveAnimal(PlayerController ColliderPlayer)
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
        if(Renderder == null)
        {
            yield return null;
        }

        Color OriginColor = Renderder.material.color;
        if (Renderder == null)
        {
            yield return null;
        }

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
        if(SpawnableObject != null)
        {
            SpawnableObject.IsDestroying = true;
        }
    }
}
