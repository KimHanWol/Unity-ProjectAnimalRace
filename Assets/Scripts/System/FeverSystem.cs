using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class FeverSystem : MonoBehaviour
{
    [Header("Ready")]
    public float FirstDelay = 1f;
    public float GrowDuration = 1f;
    public float DelayAfterGrown = 1f;
    public float TurnDuration = 1f;
    public string ReadyEmojiKey;
    public float EmojiDuration = 2f;
    public float LastDuration = 1f;

    [Header("FeverTime")]
    public float FeverTimeDuration = 1f;
    public float MaxSizeScale = 3f;
    public float CameraShakeVelocity = 0.05f;
    private ShakeComponent CameraShakeComponent;
    private ShakeComponent ScoreUIShakeComponent;
    public float ScoreColorChangeSpeed = 1f;
    public Text ScoreInGameText;
    public Text ScoreInGameFeverText;
    private Color ScoreInGameTextDefaultColor;
    public float FeverTimeAdditionalScoreRate = 1f;

    [Header("Fever Finish")]
    public float FinishFirstDelay = 1f;
    public float ShrinkDuration = 1f;
    public float DelayAfterShrink = 1f;
    public string FinishEmojiKey;
    public float FinishEmojiDuration = 2f;
    public float FinishLastDuration = 1f;

    private FeverInterface PlayerFeverInterface;
    private FeverInterface HunterFeverInterface;

    private bool IsInFeverTime = false;

    private void Awake()
    {
        EventManager.Instance.OnFeverStateChangedEvent.AddListener(OnFeverStateChanged);
    }

    private void Start()
    {
        GameObject PlayerObject = GameManager.Instance.Player.gameObject;
        PlayerFeverInterface = PlayerObject.GetComponent<FeverInterface>();

        GameObject HunterObject = GameManager.Instance.Hunter.gameObject;
        HunterFeverInterface = HunterObject.GetComponent<FeverInterface>();

        CameraShakeComponent = GameManager.Instance.MainCamera.GetComponentInChildren<ShakeComponent>();
        ScoreUIShakeComponent = UIManager.Instance.ScoreInGameUI.GetComponentInChildren<ShakeComponent>();

        ScoreInGameTextDefaultColor = ScoreInGameText.color;
    }

    private void OnFeverStateChanged(bool Enabled)
    {
        if (Enabled == true)
        {
            StartCoroutine(ReadyFever());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator ReadyFever()
    {
        // Play fever bgm
        SoundManager SoundManager = SoundManager.Instance;
        SoundManager.StopBGM(true);

        PlayerFeverInterface.FeverInitialize();
        HunterFeverInterface.FeverInitialize();

        PlayerFeverInterface.FeverReadyForStart(FirstDelay, GrowDuration, DelayAfterGrown, TurnDuration, ReadyEmojiKey, EmojiDuration, LastDuration);
        HunterFeverInterface.FeverReadyForStart(FirstDelay, GrowDuration, DelayAfterGrown, TurnDuration, ReadyEmojiKey, EmojiDuration, LastDuration);

        float ReadyDuration = FirstDelay + GrowDuration + DelayAfterGrown + TurnDuration + EmojiDuration + LastDuration;
        yield return new WaitForSeconds(ReadyDuration);

        // Play fever bgm
        SoundManager.PlayBGM(SoundManager.EBGM.BGM_FEVER, true);

        // Fever Time
        IsInFeverTime = true;
        PlayerFeverInterface.FeverStart();
        HunterFeverInterface.FeverStart();
        StartCoroutine(ShakeCameraWhenPlayerMoves());
        StartCoroutine(ChangeColorOfScoreText());
        ScoreInGameFeverText.gameObject.SetActive(true);
        ScoreUIShakeComponent.EnableShake(ScoreUIShakeComponent.transform.parent.gameObject, true);
        yield return new WaitForSeconds(FeverTimeDuration);
        // ~Fever Time

        // Play normal bgm
        SoundManager.StopBGM(true);

        IsInFeverTime = false;
        PlayerFeverInterface.FeverReadyForFinish(FinishFirstDelay, ShrinkDuration, DelayAfterShrink, FinishEmojiKey, FinishEmojiDuration, FinishLastDuration);
        HunterFeverInterface.FeverReadyForFinish(FinishFirstDelay, ShrinkDuration, DelayAfterShrink, FinishEmojiKey, FinishEmojiDuration, FinishLastDuration);
        ScoreInGameText.color = ScoreInGameTextDefaultColor;
        ScoreInGameFeverText.gameObject.SetActive(false);
        ScoreUIShakeComponent.EnableShake(ScoreUIShakeComponent.transform.parent.gameObject, false);
        yield return new WaitForSeconds(FinishFirstDelay + ShrinkDuration + DelayAfterShrink + FinishEmojiDuration + FinishLastDuration);

        PlayerFeverInterface.FeverFinished();
        HunterFeverInterface.FeverFinished();

        SoundManager.PlayBGM(SoundManager.EBGM.BGM_PLAYING, true);

        EventManager.Instance.OnFeverStateChangedEvent?.Invoke(false);
    }

    private IEnumerator ShakeCameraWhenPlayerMoves()
    {
        PlayerController Player = GameManager.Instance.Player;
        GameObject MainCameraObject = GameManager.Instance.MainCamera.gameObject;

        bool IsPlayerMoved = false;
        while (IsInFeverTime == true)
        {
            bool IsPlayerMoving = Mathf.Abs(Player.CurrentVelocity) >= CameraShakeVelocity;
            if (IsPlayerMoved != IsPlayerMoving)
            {
                IsPlayerMoved = IsPlayerMoving;
                CameraShakeComponent.EnableShake(MainCameraObject, IsPlayerMoving);
            }

            yield return new WaitForFixedUpdate();
        }

        CameraShakeComponent.EnableShake(GameManager.Instance.MainCamera.gameObject, false);
    }

    private IEnumerator ChangeColorOfScoreText()
    {
        while (IsInFeverTime == true)
        {
            // 시간에 따라 Hue 값 변경
            float hue = Mathf.PingPong(Time.time * ScoreColorChangeSpeed, 1.0f);

            // 무지개 색 생성 및 반영
            Color rainbowColor = Color.HSVToRGB(hue, 1, 1);
            ScoreInGameText.color = rainbowColor;
            yield return new WaitForFixedUpdate();
        }

        ScoreInGameText.color = ScoreInGameTextDefaultColor;
    }

    public bool IsFeverTime()
    {
        return IsInFeverTime;
    }
}