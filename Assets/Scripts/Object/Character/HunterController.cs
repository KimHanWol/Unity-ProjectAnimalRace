using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HunterController : GameObjectController
{
    public float HunterMovementRate = 20f;

    [Header("Force")]
    public float ForceInitial;
    public float ForceIncreaseRate;

    [Header("Delay")]
    public float FirstDelay = 1f;
    public float DelayInitial = 1f;
    public float DelayDecreaseRate = 0.01f;
    public float DelayMin = 0.05f;

    public TriggerCollisionComponent CanHitCollisionCompoment;
    public TriggerCollisionComponent GameOverCollisionComponent;

    private float CurrentForceRate = 1f;
    private float CurrentDurationRate = 1f;
    private Vector2 StartPosition = Vector2.zero;

    private Rigidbody2D RigidBody2D;
    private Animator Animator;

    new void Start()
    {
        base.Start();
        RigidBody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        StartPosition = transform.position;
        EventManager.Instance.OnPlayerAcceleratedEvent.AddListener(OnAnimalAccelerated);
        EventManager.Instance.OnAnimalChangedEvent.AddListener(OnAnimalChanged);

        CanHitCollisionCompoment.OnTriggerEnter.AddListener(OnCanHit);
        CanHitCollisionCompoment.OnTriggerExit.AddListener(OnCantHit);

        GameOverCollisionComponent.OnTriggerEnter.AddListener(OnCatchAnimal);
    }

    protected override void OnPlayGame()
    {
        EnableMovement(true);
    }

    protected override void OnGameOver()
    {
        ResetGameObject();
    }

    protected override void OnAnimalTryingToChange()
    {
        IsMoveEnabled = false;
        EnableMovement(IsMoveEnabled);
    }
    
    private void OnAnimalChanged(bool IsInitializing, AnimalType NewAnimalType)
    {
        IsMoveEnabled = true;
        if(IsInitializing == false)
        {
            EnableMovement(IsMoveEnabled);
        }
    }

    protected override void ResetGameObject()
    {
        Rigidbody2D PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerRigidbody.velocity = Vector2.zero;

        CurrentForceRate = 1f;
        CurrentDurationRate = 1f;

        transform.position = StartPosition;

        Animator.SetBool("IsRunning", false);

        StopAllCoroutines();
    }

    protected void OnAnimalAccelerated(float MoveForce)
    {
        RigidBody2D.AddForce(new Vector2(-MoveForce * HunterMovementRate, 0));
    }

    protected override void EnableMovement(bool Enabled)
    {
        IsMoveEnabled = Enabled;
        if(Enabled == true)
        {
            StartCoroutine(WaitFirstDelay());
        }
        else
        {
            Animator.SetBool("IsRunning", false);
        }
    }

    private IEnumerator WaitFirstDelay()
    {
        yield return new WaitForSeconds(FirstDelay);
        StartCoroutine(MoveHunter());
    }

    private IEnumerator MoveHunter()
    {
        float CurrentDuration = Mathf.Clamp(DelayInitial * CurrentDurationRate, DelayMin, DelayInitial);
        Animator.SetBool("IsRunning", true);

        while (IsMoveEnabled == true)
        {
            Internal_MoveHunter();
            yield return new WaitForSeconds(CurrentDuration);

            CurrentDurationRate -= DelayDecreaseRate;
            CurrentForceRate += ForceIncreaseRate;
        }
    }

    private void Internal_MoveHunter()
    {
        float CurrentForce = ForceInitial * CurrentForceRate;
        RigidBody2D.AddForce(new Vector2(CurrentForce, 0));
    }

    private void OnCanHit(GameObject OverlappedGameObject)
    {
        Animator.SetBool("CanHit", true);
    }

    private void OnCantHit(GameObject OverlappedGameObject)
    {
        Animator.SetBool("CanHit", false);
    }

    private void OnCatchAnimal(GameObject OverlappedGameObject)
    {
        EventManager.Instance.OnGameOverEvent?.Invoke();
    }
}
