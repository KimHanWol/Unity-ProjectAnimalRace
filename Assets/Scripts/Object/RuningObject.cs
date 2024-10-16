using System;
using UnityEngine;

public class RuningObject : MonoBehaviour
{
    public float MovementSpeed;
    public bool IsActivated = false;
    protected bool IsNeedToStopWhenActivated = true;
    protected bool IsNeedToStopMove = false;

    void Update()
    {
        Update_ObjectMovement();
    }

    protected void Update_ObjectMovement()
    {
        if (IsNeedToStopWhenActivated == true && IsActivated == true)
        {
            return;
        }

        if (IsNeedToStopMove == true)
        {
            return;
        }

        Vector2 NewPosition = transform.position;
        NewPosition.x -= MovementSpeed;
        transform.position = NewPosition;
    }

    public virtual void EnableMovement(bool Enabled)
    {
        IsNeedToStopMove = !Enabled;
    }
}
