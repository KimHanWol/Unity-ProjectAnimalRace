using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EventManager : SingletonObject<EventManager>
{
    //Game
    public UnityEvent OnGameStartEvent;
    public UnityEvent OnGameOverEvent;
    public UnityEvent OnAnimalTryingToChangeEvent;
    public UnityEvent<AnimalType> OnAnimalTypeChangedEvent;

    //Player
    public UnityEvent<float> OnPlayerAcceleratedEvent;
    public UnityEvent<bool> OnPlayerMovementEnableChangedEvent;
}
