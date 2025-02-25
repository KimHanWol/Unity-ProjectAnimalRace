using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCollisionComponent : MonoBehaviour
{
    public string CollisionTag = "Player";

    public UnityEvent<GameObject> OnTriggerEnter;
    public UnityEvent<GameObject> OnTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != CollisionTag)
        {
            return;
        }

        OnTriggerEnter?.Invoke(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != CollisionTag)
        {
            return;
        }

        OnTriggerExit?.Invoke(collision.gameObject);
    }
}