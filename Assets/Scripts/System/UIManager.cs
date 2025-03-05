using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonObject<UIManager>
{
    public GameObject TitleUI;
    public GameObject StartingUI;
    public GameObject StartPlayUI;
    public GameObject PressAnyButtonUI;
    public GameObject SettingButtonUI;
    public GameObject UnlockedAnimalButtonUI;
    public GameObject ScoreBoardUI;
    public GameObject ScoreInGameUI;
    public GameObject SettingUI;
    public GameObject KeyGuideUI;
    public GameObject NewAnimalUI;
    public GameObject UnlockedAnimalUI;
    public GameObject UnlockedAnimalPanelPrefab;

    private bool IsWaitingInput = false;
    private bool IsStarted = false;

    public float PressAnyButtonBlinkingDuration = 1f;
    private float PressAnyButtonCurrentTime;
    public float TitleFadeDuration = 1f;
    private float CurrentTitleFadeDuration;
    private bool IsAnyButtonPressed = false;
    private float TitleAlpha = 1f;

    protected override void Awake()
    {
        base.Awake();

        EventManager.Instance.OnPlaySFXPlayedEvent.AddListener(OnRunTitle);
        EventManager.Instance.OnNewAnimalUnlockStartEvent.AddListener(OnNewAnimalUnlocked);
        SaveSystem.Instance.OnSaveDataLoadedEvent.AddListener(OnSaveDataLoaded);
    }

    void Start()
    {
        OnGameStart(false);
    }

    private void OnSaveDataLoaded(SaveData LoadedSaveData)
    {
        Slider[] SliderList = FindObjectsOfType<Slider>(true);
        foreach (Slider Slider in SliderList)
        {
            if (Slider.name == "Slider_BGM")
            {
                Slider.value = LoadedSaveData.BGMVolume;
            }
            else if (Slider.name == "Slider_SFX")
            {
                Slider.value = LoadedSaveData.SFXVolume;
            }
        }
    }

    void Update()
    {
        Update_PressAnyButtonBlinking();
    }

    private void Update_PressAnyButtonBlinking()
    {
        if (IsWaitingInput == false)
        {
            PressAnyButtonCurrentTime = 0f;
            return;
        }

        if (IsAnyButtonPressed == true)
        {
            PressAnyButtonCurrentTime = 0f;
            return;
        }

        if (IsStarted == true)
        {
            PressAnyButtonCurrentTime = 0f;
            return;
        }

        if (PressAnyButtonBlinkingDuration < PressAnyButtonCurrentTime)
        {
            PressAnyButtonUI.SetActive(!PressAnyButtonUI.activeInHierarchy);
            PressAnyButtonCurrentTime = 0f;
        }

        PressAnyButtonCurrentTime += Time.deltaTime;
    }

    public void OnAnyButtonPressed()
    {
        IsAnyButtonPressed = true;
    }

    public void PlayTitleFadeAnimation(bool IsFadeIn)
    {
        IsAnyButtonPressed = true;

        PressAnyButtonUI.SetActive(IsFadeIn);
        SettingButtonUI.SetActive(IsFadeIn);
        UnlockedAnimalButtonUI.SetActive(IsFadeIn);

        StopAllCoroutines();
        StartCoroutine(Loop_TitleFadeAnimation(IsFadeIn));
    }

    IEnumerator Loop_TitleFadeAnimation(bool IsFadeIn)
    {
        CurrentTitleFadeDuration = TitleAlpha / 1f * TitleFadeDuration;
        while (true)
        {
            if (IsFadeIn == true)
            {
                CurrentTitleFadeDuration += Time.deltaTime;
            }
            else
            {
                CurrentTitleFadeDuration -= Time.deltaTime;
            }

            CurrentTitleFadeDuration = Mathf.Clamp(CurrentTitleFadeDuration, 0f, TitleFadeDuration);

            Text[] TextArray = TitleUI.GetComponentsInChildren<Text>();
            Image[] ImageArray = TitleUI.GetComponentsInChildren<Image>();

            TitleAlpha = CurrentTitleFadeDuration / TitleFadeDuration;
            TitleAlpha = Mathf.Clamp(TitleAlpha, 0f, 1f);

            foreach (Text Text in TextArray)
            {
                Color NewColor = new Color(Text.color.r, Text.color.g, Text.color.b, TitleAlpha);
                Text.color = NewColor;
            }
            foreach (Image Image in ImageArray)
            {
                float InTitleAlpha = TitleAlpha;
                if (IsFadeIn == true)
                {
                    InTitleAlpha = TitleAlpha > 0.5f ? TitleAlpha : 0f;
                }
                else
                {
                    InTitleAlpha = TitleAlpha - 0.5f;
                }

                Color NewColor = new Color(Image.color.r, Image.color.g, Image.color.b, InTitleAlpha);
                Image.color = NewColor;
            }

            if (IsFadeIn == true && TitleAlpha == 1f ||
                IsFadeIn == false && TitleAlpha == 0f)
            {
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void OnGameStart(bool InputEnabled)
    {
        TitleUI.SetActive(true);
        SettingButtonUI.SetActive(true);
        UnlockedAnimalButtonUI.SetActive(true);
        ScoreBoardUI.SetActive(false);
        ScoreInGameUI.SetActive(false);

        IsWaitingInput = InputEnabled;
        PressAnyButtonUI.SetActive(InputEnabled);
    }

    public void OnPlayStarting()
    {
        IsStarted = true;

        TitleUI.SetActive(false);
        PressAnyButtonUI.SetActive(false);
        SettingButtonUI.SetActive(false);
        UnlockedAnimalButtonUI.SetActive(false);

        StartingUI.SetActive(true);
        StartPlayUI.SetActive(false);
    }

    public void OnRunTitle()
    {
        StartingUI.SetActive(false);
        StartPlayUI.SetActive(true);
        StartCoroutine(WaitStartPlayUITimer(0.7f));
    }

    public void OnPlaying()
    {
        ScoreInGameUI.SetActive(true);
    }

    public void OnGameOver(int NewScore, bool IsNewRecord)
    {
        //TODO: New Record 효과 추가하기

        ScoreBoardUI.SetActive(true);
        Text[] ScoreboardUITexts = ScoreBoardUI.GetComponentsInChildren<Text>();
        if (ScoreboardUITexts.Length > 1)
        {
            ScoreboardUITexts[1].text = NewScore.ToString() + "m";
        }

        ScoreInGameUI.SetActive(false);
    }

    private void OnNewAnimalUnlocked(AnimalType NewAnimalType)
    {
        AnimalData TargetAnimalData = AnimalDataManager.Instance.GetAnimalData(NewAnimalType);

        NewAnimalUI.SetActive(true);

        // 애니메이션 교체
        Animator Animator_Animal = NewAnimalUI.transform.Find("AnimalPanel").GetComponentInChildren<Animator>();
        Animator_Animal.runtimeAnimatorController = TargetAnimalData.Animator;
        Animator_Animal.speed = 0.5f;
        Animator_Animal.enabled = true;
        StartCoroutine(PlayNewAnimalAnimation());

        // 이름 변경
        Text Text_Name = NewAnimalUI.transform.Find("AnimalPanel").GetComponentInChildren<Text>();
        Text_Name.text = TargetAnimalData.AnimalType.ToString();

        // 키 가이드 설정
        Animator KeyGuideAnimator = NewAnimalUI.transform.Find("KeyGuidePanel").GetComponentInChildren<Animator>();
        KeyGuideAnimator.enabled = true;
        AnimationClip InputAnimationClip = AnimalDataManager.Instance.GetInputTypeAnimationClip(TargetAnimalData.InputType);
        KeyGuideAnimator.Play(InputAnimationClip.name, 0, 0f);
    }

    IEnumerator PlayNewAnimalAnimation()
    {
        Image AnimalImage = NewAnimalUI.transform.Find("AnimalPanel").Find("Animator_Animal").GetComponentInChildren<Image>();
        SpriteRenderer SpriteRenderer = NewAnimalUI.transform.Find("AnimalPanel").GetComponentInChildren<SpriteRenderer>();
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            AnimalImage.sprite = SpriteRenderer.sprite;
        }
    }

    public void OnNewAnimalUIButtonClicked()
    {
        StopCoroutine(PlayNewAnimalAnimation());

        Animator Animator_Animal = NewAnimalUI.transform.Find("AnimalPanel").GetComponentInChildren<Animator>();
        Animator_Animal.enabled = false;

        Animator KeyGuideAnimator = NewAnimalUI.transform.Find("KeyGuidePanel").GetComponentInChildren<Animator>();
        KeyGuideAnimator.enabled = false;

        NewAnimalUI.SetActive(false);

        EventManager.Instance.OnNewAnimalUnlockFinishedEvent?.Invoke();
    }

    IEnumerator WaitStartPlayUITimer(float ShowDuration)
    {
        yield return new WaitForSeconds(ShowDuration);

        StopCoroutine(WaitStartPlayUITimer(ShowDuration));
        StartPlayUI.SetActive(false);
    }

    public void UpdateScoreData(int NewScore)
    {
        Text ScoreInGameUIText = ScoreInGameUI.GetComponentInChildren<Text>();
        ScoreInGameUIText.text = NewScore.ToString() + "m";
    }

    public void EnableSettingsPanel(bool Enabled)
    {
        SettingUI.SetActive(Enabled);
    }

    public void EnableKeyGuidePanel(bool Enabled)
    {
        KeyGuideUI.SetActive(Enabled);
    }

    public void EnableUnlockedAnimalPanel(bool Enabled)
    {
        if (Enabled == true)
        {
            InitializeUnlockedAnimalPanel();
        }

        UnlockedAnimalUI.SetActive(Enabled);
    }

    private void InitializeUnlockedAnimalPanel()
    {
        GameObject ParentGameObject = UnlockedAnimalUI.GetComponentInChildren<GridLayoutGroup>().gameObject;

        // 개수 세팅
        if(ParentGameObject.transform.childCount != AnimalDataManager.Instance.AnimalDataList.Length)
        {
            ParentGameObject.transform.DetachChildren();

            foreach (AnimalData InAnimalData in AnimalDataManager.Instance.AnimalDataList)
            {
                GameObject NewAnimalPanel = Instantiate(UnlockedAnimalPanelPrefab, ParentGameObject.transform);
                Image[] ImageAnimalArray = NewAnimalPanel.transform.GetComponentsInChildren<Image>();

                ImageAnimalArray[0].sprite = InAnimalData.AnimalPortrait;
                ImageAnimalArray[1].sprite = InAnimalData.AnimalFeverPortrait;

                Text Text_Name = NewAnimalPanel.GetComponentInChildren<Text>();
                Text_Name.text = InAnimalData.AnimalType.ToString();
            }
        }

        // 언락 데이터 업데이트
        for (int i = 0; i < ParentGameObject.transform.childCount; i++) 
        {
            GameObject AnimalPanel = ParentGameObject.transform.GetChild(i).gameObject;
            Image[] ImageAnimalArray = AnimalPanel.transform.GetComponentsInChildren<Image>();
            Text Text_Name = AnimalPanel.GetComponentInChildren<Text>();

            // Normal Animal
            {
                bool IsNormalUnlocked = false;
                foreach (AnimalType InAnimalType in AnimalDataManager.Instance.UnlockedAnimalList)
                {
                    if (InAnimalType.ToString() == Text_Name.text)
                    {
                        IsNormalUnlocked = true;
                    }
                }

                if (IsNormalUnlocked == true)
                {
                    ImageAnimalArray[0].color = Color.white;
                }
                else
                {
                    ImageAnimalArray[0].color = Color.black;
                }
            }

            // Fever Animal
            {
                bool IsFeverUnlocked = false;
                foreach (AnimalType InAnimalType in AnimalDataManager.Instance.UnlockedFeverAnimalList)
                {
                    if (InAnimalType.ToString() == Text_Name.text)
                    {
                        IsFeverUnlocked = true;
                    }
                }

                if (IsFeverUnlocked == true)
                {
                    ImageAnimalArray[1].color = Color.white;
                }
                else
                {
                    ImageAnimalArray[1].color = Color.black;
                }
            }

        }
    }

    public bool IsInputDisableUIOpened()
    {
        if(SettingUI.activeInHierarchy == true || UnlockedAnimalUI.activeInHierarchy == true)
        {
            return true;
        }

        return false;
    }
}
