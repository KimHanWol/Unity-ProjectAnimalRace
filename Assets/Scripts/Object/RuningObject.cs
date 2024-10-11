using UnityEngine;

public class RuningObject : MonoBehaviour
{
    public Vector2 MovementSpeed;
    protected bool IsActivated = false;

    void Update()
    {
        if(IsActivated == true)
        {
            return;
        }

        Vector2 NewPosition = transform.position;
        NewPosition += MovementSpeed;
        transform.position = NewPosition;
    }
}
