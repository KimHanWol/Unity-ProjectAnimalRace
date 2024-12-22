using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HunterController : MonoBehaviour
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

    private float CurrentForceRate = 1f;
    private float CurrentDurationRate = 1f;
    private Vector2 StartPosition = Vector2.zero;
    private bool IsMoveEnabled = false;

    private Rigidbody2D RigidBody2D;
    private Animator Animator;

    public UnityEvent OnAnimalTryingToChangeEvent;

    void Start()
    {
        RigidBody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        StartPosition = transform.position;
    }

    public void OnGameStart()
    {
        EnableMovement(true);
    }

    public void ResetHunter()
    {
        Rigidbody2D PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerRigidbody.velocity = Vector2.zero;

        CurrentForceRate = 1f;
        CurrentDurationRate = 1f;

        transform.position = StartPosition;

        Animator.SetBool("IsRunning", false);

        StopAllCoroutines();
    }

    public void EnableMovement(bool Enabled)
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

    public void OnPlayerAccelerated(float MoveForce)
    {
        RigidBody2D.AddForce(new Vector2(-MoveForce * HunterMovementRate, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }

        Animator.SetBool("CanHit", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }

        Animator.SetBool("CanHit", false);
    }

    public void SetIsAnimalChanging(bool IsChanging)
    {
        if (IsMoveEnabled == !IsChanging)
        {
            return;
        }
        IsMoveEnabled = !IsChanging;

        EnableMovement(IsMoveEnabled);

        if(IsChanging == true)
        {
            OnAnimalTryingToChangeEvent?.Invoke();
        }
    }
}
