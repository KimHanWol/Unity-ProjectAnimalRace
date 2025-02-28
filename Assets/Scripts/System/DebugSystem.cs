using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugSystem : MonoBehaviour
{
    // 인풋을 화면에 나타낼 것인지
    public bool EnableInputPrint;

    public Text DebugUI;
    public GameObject TitleDataResetButton;

    private void Awake()
    {
        EventManager.Instance.OnPlayGameEvent.AddListener(OnPlayGame);
        EventManager.Instance.OnGameOverEvent.AddListener(OnGameOver);
    }

    private void OnPlayGame()
    {
        TitleDataResetButton.SetActive(false);
    }

    private void OnGameOver()
    {
        TitleDataResetButton.SetActive(true);
    }

    public void ResetGame()
    {
        SaveSystem.Instance.ResetData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}