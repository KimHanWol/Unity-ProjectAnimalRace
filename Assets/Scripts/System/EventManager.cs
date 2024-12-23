using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EventManager : SingletonObject<EventManager>
{
    //Game
    public UnityEvent OnPlayGameEvent;
    public UnityEvent OnGameOverEvent;
    public UnityEvent OnAnimalTryingToChangeEvent;

    //IsInitializing, AnimalType
    public UnityEvent<bool, AnimalType> OnAnimalChangedEvent;

    //Player
    public UnityEvent<float> OnPlayerAcceleratedEvent;
}
