using System.Collections;
using UnityEngine;

public class RuningObject : MonoBehaviour, InteractableInterface
{
    public float MovementSpeedRate = 1;
    public bool IsActivated = false;
    public float DestroyDuration = 0.5f;
    protected bool IsNeedToStopWhenActivated = true;
    protected bool IsNeedToStopMove = false;

    private Rigidbody2D RigidBody2D;

    public void Start()
    {
        EventManager.Instance.OnPlayerAcceleratedEvent.AddListener(OnPlayerAccelerated);

        RigidBody2D = GetComponent<Rigidbody2D>();
    }

    void OnPlayerAccelerated(float MoveForce)
    {
        if(IsNeedToStopMove == false)
        {
            RigidBody2D.AddForce(new Vector2(-MoveForce * MovementSpeedRate, 0));
        }
    }

    public virtual void EnableMovement(bool Enabled)
    {
        IsNeedToStopMove = !Enabled;

        if(IsNeedToStopMove == true)
        {
            RigidBody2D.velocity = Vector2.zero;
        }
    }

    //InteractableInterface
    public virtual void Interaction(GameObject InteractObject) { }
    //~InteractableInterface

    private IEnumerator PlayDisappearEffect_Internal()
    {
        BoxCollider2D BoxCollider2D = GetComponent<BoxCollider2D>();
        BoxCollider2D.enabled = false;

        SpriteRenderer SpriteRenderer = GetComponent<SpriteRenderer>();

        float CurrentTime = 0f;
        while (CurrentTime < DestroyDuration)
        {
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 1f - CurrentTime / DestroyDuration);

            yield return new WaitForSeconds(0.1f);
            CurrentTime += 0.1f;
        }
        SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0f);
    }

    protected void SelfDestroy()
    {
        SpawnableComponent SpawnableObject = GetComponent<SpawnableComponent>();
        SpawnableObject.IsDestroying = true;

        StartCoroutine(PlayDisappearEffect_Internal());
    }
}
