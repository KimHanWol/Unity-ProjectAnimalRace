using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EventManager : SingletonObject<EventManager>
{
    //Game
    public UnityEvent OnStartGameEvent; //타이틀 화면 시작
    public UnityEvent OnPlayGameEvent; //달리기 시작
    public UnityEvent OnGameOverEvent; //달리기 끝
    public UnityEvent OnAnimalTryingToChangeEvent;
    public UnityEvent<bool> OnFeverStateChangedEvent;
    public UnityEvent<AnimalType> OnNewAnimalUnlockStartEvent;
    public UnityEvent OnNewAnimalUnlockFinishedEvent;

    //<IsInitializing, AnimalType>
    public UnityEvent<bool, AnimalType> OnAnimalChangedEvent;

    //Player
    public UnityEvent<float> OnPlayerAcceleratedEvent;

    //Sound
    public UnityEvent OnPlaySFXPlayedEvent;
}
