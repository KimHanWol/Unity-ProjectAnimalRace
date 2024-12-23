using UnityEngine;

public class RuningObject : MonoBehaviour
{
    public float MovementSpeedRate = 1;
    public bool IsActivated = false;
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
}
