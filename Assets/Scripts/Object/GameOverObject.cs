using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverObject : MonoBehaviour
{
    public UnityEvent OnGameOver;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Hunter")
        {
            return;
        }

        OnGameOver.Invoke();
    }
}
