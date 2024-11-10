using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject TitleUI;
    public GameObject StartingUI;
    public GameObject StartPlayUI;
    public GameObject PressAnyButtonUI;
    public GameObject SettingButtonUI;

    private bool IsStarted = false;

    public float PressAnyButtonBlinkingDuration = 1f;
    private float PressAnyButtonCurrentTime;
    public float TitleFadeDuration = 1f;
    private float CurrentTitleFadeDuration;
    private bool IsAnyButtonPressed = false;
    private float TitleAlpha = 1f;

    void Start()
    {
        OnGameStart(false);
    }

    void Update()
    {
        Update_PressAnyButtonBlinking();
    }

    private void Update_PressAnyButtonBlinking()
    {
        if(IsAnyButtonPressed == true)
        {
            return;
        }

        if(IsStarted == true)
        {
            return;
        }

        if(PressAnyButtonBlinkingDuration < PressAnyButtonCurrentTime)
        {
            if (PressAnyButtonUI != null)
            {
                PressAnyButtonUI.SetActive(!PressAnyButtonUI.activeInHierarchy);
            }

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

        if (PressAnyButtonUI != null)
        {
            PressAnyButtonUI.SetActive(IsFadeIn);
        }

        if (SettingButtonUI != null)
        {
            SettingButtonUI.SetActive(IsFadeIn);
        }

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

            if (TitleUI != null)
            {
                Text[] TextArray = TitleUI.GetComponentsInChildren<Text>();
                Image[] ImageArray = TitleUI.GetComponentsInChildren<Image>();

                TitleAlpha = CurrentTitleFadeDuration / TitleFadeDuration;
                TitleAlpha = Mathf.Clamp(TitleAlpha, 0f, 1f);

                foreach (Text Text in TextArray)
                {
                    Color NewColor = new Color(Text.color.r, Text.color.g, Text.color.b, TitleAlpha);
                    Text.color = NewColor;
                }
                foreach(Image Image in ImageArray)
                {
                    float InTitleAlpha = TitleAlpha;
                    if(IsFadeIn == true)
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
        PressAnyButtonUI.SetActive(InputEnabled);
        SettingButtonUI.SetActive(true);
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

    public void OnPlaying()
    {
        StartingUI.SetActive(false);
        StartPlayUI.SetActive(true);

        StartCoroutine(WaitStartPlayUITimer(1f));
    }

    IEnumerator WaitStartPlayUITimer(float ShowDuration)
    {
        yield return new WaitForSeconds(ShowDuration);

        StopCoroutine(WaitStartPlayUITimer(ShowDuration));
        StartPlayUI.SetActive(false);
    }
}
