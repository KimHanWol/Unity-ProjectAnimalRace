using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KeyGuideComponent : MonoBehaviour
{
    private PlayerController PlayerController;
    private GameObject KeyGuideUI;
    private Animator KeyGuideAnimator;
    private AnimationClip CurrentInputAnimationClip;

    public float KeyGuideShowDuration = 2f;
    public float KeyGuideShowSpeed = 100f;

    private AnimalType CurrentAnimalType;
    private float CurrentSpeed;

    private bool IsReadyState = true;
    private bool IsKeyGuideOnDuration = false; //지속시간 이내일 때
    private bool IsKeyGuideActivated = false;
    private bool IsSpeedFasterThanShowSpeed = false;
    private bool IsAnimalTryingToChange = false;
    private bool IsFeverState = false;

    void Awake()
    {
        PlayerController = GetComponentInParent<PlayerController>();
        KeyGuideUI = UIManager.Instance.KeyGuideUI;
        KeyGuideAnimator = KeyGuideUI.GetComponentInChildren<Animator>();
        KeyGuideAnimator.speed = 2;

        EventManager EventManager = EventManager.Instance;
        EventManager.OnPlayGameEvent.AddListener(OnPlayGame);
        EventManager.OnGameOverEvent.AddListener(OnGameOver);
        EventManager.OnAnimalTryingToChangeEvent.AddListener(OnAnimalTryingToChange);
        EventManager.OnAnimalChangedEvent.AddListener(OnAnimalTypeChanged);
        EventManager.OnFeverStateChangedEvent.AddListener(OnFeverStateChanged);
    }

    void OnPlayGame()
    {
        IsReadyState = false;
    }

    void OnGameOver()
    {
        IsReadyState = true;
        EnableKeyGuide(false);
    }

    void OnAnimalTryingToChange()
    {
        IsAnimalTryingToChange = true;
        EnableKeyGuide(false);
    }

    void OnAnimalTypeChanged(bool IsInitializing, AnimalType NewAnimalType)
    {
        CurrentAnimalType = NewAnimalType;
        IsAnimalTryingToChange = false;

        InputType CurrentInputType = PlayerController.GetCurrentAnimalInputType();
        AnimalDataManager AnimalDataManager = AnimalDataManager.Get();
        CurrentInputAnimationClip = AnimalDataManager.GetInputTypeAnimationClip(CurrentInputType);
        if(IsKeyGuideActivated == true)
        {
            KeyGuideAnimator.Play(CurrentInputAnimationClip.name, 0, 0f);
        }
    }

    void OnFeverStateChanged(bool Enabled)
    {
        IsFeverState = Enabled;
        EnableKeyGuide(IsFeverState == false);
    }

    bool IsKeyGuideNeedToBeEnabled()
    {
        // 현재 Duration 이내이면 조건 확인하지 않고 항상 true
        if (IsKeyGuideOnDuration == true)
        {
            return true;
        }

        // 변하는 와중에는 보여주지 않음
        if (IsAnimalTryingToChange == true)
        {
            return false;
        }

        // 준비중인 상태에서도 보여주지 않음
        if (IsReadyState == true)
        {
            return false;
        }

        // 속도가 충분히 빠르면 보여주지 않음
        if(IsSpeedFasterThanShowSpeed == true)
        {
            return false;
        }

        // 피버 타임에는 보여주지 않음
        if(IsFeverState == true)
        {
            return false;
        }

        return true;
    }

    void Update()
    {
        CurrentSpeed = PlayerController.CurrentVelocity;
        IsSpeedFasterThanShowSpeed = CurrentSpeed > KeyGuideShowSpeed;

        bool CanKeyGuideShow = IsKeyGuideNeedToBeEnabled();
        EnableKeyGuide(CanKeyGuideShow);

        if (IsKeyGuideActivated == true)
        {
            Update_SetKeyGuideLocation();
        }
    }

    void Update_SetKeyGuideLocation()
    {
        if (KeyGuideUI.activeInHierarchy == false)
        {
            return;
        }

        Vector3 PlayerPosition = PlayerController.gameObject.transform.position;
        Vector3 NewKeyGuidePosition = Camera.main.WorldToScreenPoint(PlayerPosition);
        NewKeyGuidePosition.y = KeyGuideUI.transform.position.y;

        KeyGuideUI.transform.position = NewKeyGuidePosition;
    }

    IEnumerator Timer_KeyGuideShow()
    {
        IsKeyGuideOnDuration = true;
        yield return new WaitForSeconds(KeyGuideShowDuration);
        IsKeyGuideOnDuration = false;
    }

    private void EnableKeyGuide(bool Enabled)
    {
        if (Enabled == true)
        {
            //이미 켜진 상태면 재활성화 하지 않음
            if(IsKeyGuideActivated == true)
            {
                return;
            }

            KeyGuideUI.SetActive(true);
            KeyGuideAnimator.enabled = true;
            KeyGuideAnimator.Play(CurrentInputAnimationClip.name, 0, 0f);
            StartCoroutine(Timer_KeyGuideShow());
        }
        else
        {
            KeyGuideAnimator.enabled = false;
            KeyGuideUI.SetActive(false);
            StopCoroutine(Timer_KeyGuideShow());
            IsKeyGuideOnDuration = false;
        }

        IsKeyGuideActivated = Enabled;
    }
}
