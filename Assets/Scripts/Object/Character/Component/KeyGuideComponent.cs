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
    private bool IsKeyGuideOnDuration = false; //���ӽð� �̳��� ��
    private bool IsKeyGuideActivated = false;
    private bool IsSpeedFasterThanShowSpeed = false;
    private bool IsAnimalTryingToChange = false;

    void Awake()
    {
        PlayerController = this.gameObject.GetComponent<PlayerController>();
        KeyGuideUI = UIManager.Instance.KeyGuideUI;
        KeyGuideAnimator = KeyGuideUI.GetComponentInChildren<Animator>();
        KeyGuideAnimator.speed = 2;

        PlayerController.OnGameStartEvent.AddListener(OnGameStart);
        PlayerController.OnGameOverEvent.AddListener(OnGameOver);
        PlayerController.OnAnimalTryingToChangeEvent.AddListener(OnAnimalTryingToChange);
        PlayerController.OnAnimalTypeChangedEvent.AddListener(OnAnimalTypeChanged);
    }

    void OnGameStart()
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

    void OnAnimalTypeChanged(AnimalType NewAnimalType)
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

    bool IsKeyGuideNeedToBeEnabled()
    {
        // ���� Duration �̳��̸� ���� Ȯ������ �ʰ� �׻� true
        if (IsKeyGuideOnDuration == true)
        {
            return true;
        }

        // ���ϴ� ���߿��� �������� ����
        if (IsAnimalTryingToChange == true)
        {
            return false;
        }

        // �غ����� ���¿����� �������� ����
        if (IsReadyState == true)
        {
            return false;
        }

        // �ӵ��� ����� ������ �������� ����
        if(IsSpeedFasterThanShowSpeed == true)
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
            //�̹� ���� ���¸� ��Ȱ��ȭ ���� ����
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
