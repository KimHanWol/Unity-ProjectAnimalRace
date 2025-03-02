using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : SingletonObject<GameManager>
{
    // GameObject
    [Header("Object")]
    public PlayerController Player;
    public HunterController Hunter;
    public Camera MainCamera;
    public ObjectSpawner ObjectSpawner;
    public MapManager MapManager;
    public UIManager UIManager;
    public SoundManager SoundManager;

    [Header("Data")]
    public AnimalType DefaultAnimalType;

    // 타이틀이 나오고 입력을 받지 않는 시간
    public float TitleNoInputDuration = 2f;

    [Header("Score")]
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
        EventManager.Instance.OnStartGameEvent.AddListener(OnGameStart);
        SaveSystem.Instance.OnSaveDataLoadedEvent.AddListener(OnSaveDataLoaded);

        SoundManager.OnBGMChanged.AddListener(OnBGMChanged);
    }

    void Start()
    {
        //Data Load
        SaveSystem.Instance.LoadData();

        OnGameStart();

        //Full HD
        Screen.SetResolution(1920, 1080, false);
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

        if (Input.anyKeyDown == true)
        {
            // 다른 오브젝트(버튼) 를 좌클릭했을 때는 무시
            if (Input.GetKey(KeyCode.Mouse0) == true)
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    return;
                }
            }

            GameState = EGameState.State_Starting;
            SoundManager.PlayBGM(SoundManager.EBGM.BGM_PLAYING, false);
            UIManager.OnPlayStarting();
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
