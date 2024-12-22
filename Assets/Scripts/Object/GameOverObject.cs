using UnityEngine;
using UnityEngine.Events;

public class GameOverObject : MonoBehaviour
{
    public UnityEvent OnGameOverEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Hunter")
        {
            return;
        }

        OnGameOverEvent.Invoke();
    }
}
