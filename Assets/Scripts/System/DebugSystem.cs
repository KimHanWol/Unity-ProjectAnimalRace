using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugSystem : MonoBehaviour
{
    // 인풋을 화면에 나타낼 것인지
    public bool EnableInputPrint;

    public Text DebugUI;
    public GameObject TitleDataResetButton;

    private string DefaultDebugString;

    private void Awake()
    {
        EventManager.Instance.OnPlayGameEvent.AddListener(OnPlayGame);
        EventManager.Instance.OnGameOverEvent.AddListener(OnGameOver);
    }

    private void Start()
    {
        DefaultDebugString = DebugUI.text;
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

    private void Update()
    {
        // Default 출력
        string DebugString = DefaultDebugString + "\n";

        // 획득한 동물 수
        DebugString +=
            "Animal collection ( " + AnimalDataManager.Instance.UnlockedAnimalList.Count +
            " / " + AnimalDataManager.Instance.AnimalDataList.Length + " )\n";

        // 획득한 동물 목록
        foreach(AnimalType UnlockedAnimalType in AnimalDataManager.Instance.UnlockedAnimalList)
        {
            DebugString += UnlockedAnimalType.ToString() + " ";
        }
        DebugString += "\n";

        // 현재 속도
        DebugString +=
            "Current Speed : " + GameManager.Instance.Player.CurrentVelocity + "\n";

        // 최고 기록
        DebugString +=
            "High Score : " + GameManager.Instance.HighScore + "m\n";

        DebugUI.text = DebugString;
    }
}