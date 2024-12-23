using System.Collections;
using UnityEngine;

public class GameManager : SingletonObject<GameManager>
{
    // GameObject
    public PlayerController Player;
    public HunterController Hunter;
    public ObjectSpawner ObjectSpawner;
    public MapManager MapManager;
    public UIManager UIManager;
    public SoundManager SoundManager;

    public RuningObject[] SpawnedObjectList;
    public GameOverObject GameOverObject;

    // 타이틀이 나오고 입력을 받지 않는 시간
    public float TitleNoInputDuration = 2f;

    private float GameScore = 0;

    enum EGameState
    {
        State_Title_NoInput,
        State_Title_Input,
        State_Starting,
        State_Playing,
    }

    private EGameState GameState;

    void Start()
    {
        SoundManager.PlayBGM(SoundManager.EBGM.BGM_START, true);

        EventManager.Instance.OnGameOverEvent.AddListener(OnGameOver);

        OnGameStart();
    }

    void Update()
    {
        Update_CheckSpeed();
        Update_WaitForAnyButtonPressed();
        Update_GameScore();
    }

    public void Update_CheckSpeed()
    {
        if (Player == null)
        {
            return;
        }

        float CurrentVelocity = Player.GetVelocity();
        MapManager.UpdateSpeed(CurrentVelocity);
    }

    private void Update_WaitForAnyButtonPressed()
    {
        if (GameState != EGameState.State_Title_Input)
        {
            return;
        }

        //왼쪽 마우스 클릭은 무시
        if (Input.anyKeyDown == true && Input.GetKey(KeyCode.Mouse0) == false)
        {
            GameState = EGameState.State_Starting;
            SoundManager.PlayBGM(SoundManager.EBGM.BGM_PLAYING, false);
            SoundManager.OnBGMChanged.AddListener(OnBGMChanged);
            UIManager.OnStarting();
        }
    }

    private void Update_GameScore()
    {
        if (GameState != EGameState.State_Playing)
        {
            return;
        }

        float CurrentVelocity = Player.GetVelocity();

        GameScore += CurrentVelocity * Time.deltaTime * 100f;

        UIManager.UpdateScoreData((int)GameScore);
    }

    private void OnBGMChanged(SoundManager.EBGM NewBGM)
    {
        if (NewBGM != SoundManager.EBGM.BGM_PLAYING)
        {
            return;
        }

        SoundManager.OnBGMChanged.RemoveListener(OnBGMChanged);
        OnPlay();
    }

    private void OnAnimalChangeEffectStateChanged(bool Enabled)
    {
        // Object Spawner
        foreach (RuningObject SpawnedObject in SpawnedObjectList)
        {
            SpawnedObject.EnableMovement(Enabled);
        }

        // Map Manager
        MapManager.EnableMovement(Enabled);
    }

    public void OnGameStart()
    {
        GameScore = 0;
        UIManager.UpdateScoreData((int)GameScore);
        UIManager.OnGameStart(false);

        GameState = EGameState.State_Title_NoInput;
        StartCoroutine(WaitTitleReady());

        SoundManager.PlayBGM(SoundManager.EBGM.BGM_START, true);
    }

    private void OnPlay()
    {
        GameState = EGameState.State_Playing;

        UIManager.OnAnyButtonPressed();
        UIManager.OnPlaying();

        EventManager.Instance.OnPlayGameEvent.Invoke();
    }

    private void OnGameOver()
    {
        UIManager.OnGameOver((int)GameScore);

        SoundManager.PlayBGM(SoundManager.EBGM.BGM_GAMEOVER, true);
    }

    IEnumerator WaitTitleReady()
    {
        yield return new WaitForSeconds(2f);

        GameState = EGameState.State_Title_Input;
        UIManager.OnGameStart(true);
    }
}
