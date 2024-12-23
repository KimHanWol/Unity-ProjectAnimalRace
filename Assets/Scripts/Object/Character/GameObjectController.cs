using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectController : MonoBehaviour
{
    protected bool IsMoveEnabled = false;

    protected void Start()
    {
        EventManager EventManager = EventManager.Instance;
        EventManager.OnPlayGameEvent.AddListener(OnPlayGame);
        EventManager.OnGameOverEvent.AddListener(OnGameOver);
        EventManager.OnAnimalTryingToChangeEvent.AddListener(OnAnimalTryingToChange);
    }

    protected virtual void OnPlayGame() { }
    protected virtual void OnGameOver() { }
    protected virtual void OnAnimalTryingToChange() { }

    protected virtual void ResetGameObject() { }
    protected virtual void EnableMovement(bool Enabled) { }
}
