using UnityEngine;
using UnityEngine.Events;

public class GameOverObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Hunter")
        {
            return;
        }

        EventManager.Instance.OnGameOverEvent?.Invoke();
    }
}
