using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonObject<UIManager>
{
    public GameObject TitleUI;
    public GameObject StartingUI;
    public GameObject StartPlayUI;
    public GameObject PressAnyButtonUI;
    public GameObject SettingButtonUI;
    public GameObject ScoreBoardUI;
    public GameObject ScoreInGameUI;
    public GameObject SettingUI;
    public GameObject KeyGuideUI;

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
        //PlayTitleFadeAnimation(false);
    }

    public void PlayTitleFadeAnimation(bool IsFadeIn)
    {
        IsAnyButtonPressed = true;

        PressAnyButtonUI.SetActive(IsFadeIn);

        SettingButtonUI.SetActive(IsFadeIn);

        StopCoroutine(Loop_TitleFadeAnimation(IsFadeIn));
        StopCoroutine(Loop_TitleFadeAnimation(!IsFadeIn));
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
        ScoreBoardUI.SetActive(false);
        ScoreInGameUI.SetActive(false);

        IsWaitingInput = InputEnabled;
        PressAnyButtonUI.SetActive(InputEnabled);
    }

    public void OnStarting()
    {
        IsStarted = true;

        TitleUI.SetActive(false);
        PressAnyButtonUI.SetActive(false);
        SettingButtonUI.SetActive(false);

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

    public void OnGameOver(int NewScore)
    {
        ScoreBoardUI.SetActive(true);
        Text[] ScoreboardUITexts = ScoreBoardUI.GetComponentsInChildren<Text>();
        if (ScoreboardUITexts.Length > 1)
        {
            ScoreboardUITexts[1].text = NewScore.ToString() + "m";
        }

        ScoreInGameUI.SetActive(false);
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

    public void EnableSettingsPanel(bool Eanbled)
    {
        SettingUI.SetActive(Eanbled);
    }

    public void EnableKeyGuidePanel(bool Eanbled)
    {
        KeyGuideUI.SetActive(Eanbled);
    }
}
