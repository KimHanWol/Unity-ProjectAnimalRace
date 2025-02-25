using UnityEngine;

public class MoveToPlayerComponent : MonoBehaviour
{
    public float MovementSpeedRate = 1;

    private Rigidbody2D RigidBody2D;
    private bool IsMovementEnabled = false;

    private void Awake()
    {
        EventManager.Instance.OnPlayerAcceleratedEvent.AddListener(OnPlayerAccelerated);
    }

    public void Start()
    {
        RigidBody2D = GetComponentInParent<Rigidbody2D>();

        IsMovementEnabled = true;
    }

    private void OnPlayerAccelerated(float MoveForce)
    {
        if (IsMovementEnabled == true)
        {
            RigidBody2D.AddForce(new Vector2(-MoveForce * MovementSpeedRate, 0));
        }
    }

    public void EnableMovement(bool IsEnabled)
    {
        IsMovementEnabled = IsEnabled;
    }
}