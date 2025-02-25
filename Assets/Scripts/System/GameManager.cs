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

    public SpawnableObject[] SpawnedObjectList;

    // 타이틀이 나오고 입력을 받지 않는 시간
    public float TitleNoInputDuration = 2f;

    public float GameScore = 0;
    public int HighScore = 0;

    enum EGameState
    {
        State_Title_NoInput,
        State_Title_Input,
        State_Starting,
        State_Playing,
    }

    private EGameState GameState;

    protected override void Awake()
    {
        base.Awake();

        EventManager.Instance.OnGameOverEvent.AddListener(OnGameOver);
        SaveSystem.Instance.OnSaveDataLoadedEvent.AddListener(OnSaveDataLoaded);
    }

    void Start()
    {
        //Data Load
        SaveSystem.Instance.LoadData();

        OnGameStart();
    }

    void OnSaveDataLoaded(SaveData LoadedSaveData)
    {
        HighScore = LoadedSaveData.HighScore;
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

        float CurrentVelocity = Mathf.Abs(Player.GetVelocity());

        //TODO: 피버 타임이면 점수 더 빠르게 오르기

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
        bool IsNewRecord = (int)GameScore > HighScore;
        if (IsNewRecord == true)
        {
            HighScore = (int)GameScore;
            SaveSystem.Instance.SaveData();
        }

        UIManager.OnGameOver((int)GameScore, IsNewRecord);
        SoundManager.PlayBGM(SoundManager.EBGM.BGM_GAMEOVER, true);

        
    }

    IEnumerator WaitTitleReady()
    {
        yield return new WaitForSeconds(2f);

        GameState = EGameState.State_Title_Input;
        UIManager.OnGameStart(true);
    }
}
