using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Fever Finish")]
    public float FinishFirstDelay = 1f;
    public float ShrinkDuration = 1f;
    public float DelayAfterShrink = 1f;
    public string FinishEmojiKey;
    public float FinishEmojiDuration = 2f;
    public float FinishLastDuration = 1f;

    private FeverInterface PlayerFeverInterface;
    private FeverInterface HunterFeverInterface;

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
        PlayerFeverInterface.FeverInitialize();
        HunterFeverInterface.FeverInitialize();
        yield return new WaitForSeconds(FirstDelay);

        PlayerFeverInterface.FeverReadyForStart(FirstDelay, GrowDuration, DelayAfterGrown, TurnDuration, ReadyEmojiKey, EmojiDuration, LastDuration);
        HunterFeverInterface.FeverReadyForStart(FirstDelay, GrowDuration, DelayAfterGrown, TurnDuration, ReadyEmojiKey, EmojiDuration, LastDuration);

        float ReadyDuration = FirstDelay + GrowDuration + DelayAfterGrown + TurnDuration + EmojiDuration + LastDuration;
        yield return new WaitForSeconds(ReadyDuration);

        // Fever Time
        PlayerFeverInterface.FeverStart();
        HunterFeverInterface.FeverStart();
        StartCoroutine(ShakeCameraWhenPlayerMoves());
        ScoreUIShakeComponent.EnableShake(ScoreUIShakeComponent.transform.parent.gameObject, true);
        yield return new WaitForSeconds(FeverTimeDuration);
        // ~Fever Time

        PlayerFeverInterface.FeverReadyForFinish(FinishFirstDelay, ShrinkDuration, DelayAfterShrink, FinishEmojiKey, FinishEmojiDuration, FinishLastDuration);
        HunterFeverInterface.FeverReadyForFinish(FinishFirstDelay, ShrinkDuration, DelayAfterShrink, FinishEmojiKey, FinishEmojiDuration, FinishLastDuration);
        StopCoroutine(ShakeCameraWhenPlayerMoves());
        CameraShakeComponent.EnableShake(GameManager.Instance.MainCamera.gameObject, false);
        ScoreUIShakeComponent.EnableShake(ScoreUIShakeComponent.transform.parent.gameObject, false);
        yield return new WaitForSeconds(FinishFirstDelay + ShrinkDuration + DelayAfterShrink + FinishEmojiDuration + FinishLastDuration);

        PlayerFeverInterface.FeverFinished();
        HunterFeverInterface.FeverFinished();

        EventManager.Instance.OnFeverStateChangedEvent?.Invoke(false);
    }

    private IEnumerator ShakeCameraWhenPlayerMoves()
    {
        PlayerController Player = GameManager.Instance.Player;
        GameObject MainCameraObject = GameManager.Instance.MainCamera.gameObject;

        bool IsPlayerMoved = false;
        while(true)
        {
            bool IsPlayerMoving = Mathf.Abs(Player.CurrentVelocity) >= CameraShakeVelocity;
            if (IsPlayerMoved != IsPlayerMoving)
            {
                IsPlayerMoved = IsPlayerMoving;
                CameraShakeComponent.EnableShake(MainCameraObject, IsPlayerMoving);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}