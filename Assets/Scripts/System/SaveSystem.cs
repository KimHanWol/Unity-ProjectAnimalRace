using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SaveData
{
    public SaveData()
    {
        UnlockedAnimalList = new List<AnimalType>();
        UnlockedAnimalList.Add(AnimalType.Dog);

        BGMVolume = 0.7f;
        SFXVolume = 0.7f;

        HighScore = 0;
    }

    //Unlocked Animal
    public List<AnimalType> UnlockedAnimalList;

    //Sound Setting
    public float BGMVolume;
    public float SFXVolume;

    //Score
    public int HighScore;
}

public class SaveSystem : SingletonObject<SaveSystem>
{
    private SaveData CurrentSaveData;

    public UnityEvent<SaveData> OnSaveDataLoadedEvent;

    //TODO: 싱글톤 말고 글로벌 이벤트로 저장하고 불러오기 할까

    // 이벤트를 직접 받지 않고, 다른 시스템에서 이벤트 관련 로직을 처리한 다음에
    // 싱글톤으로 SaveData 를 호출, 모든 데이터를 업데이트한 다음 저장
    public void SaveData()
    {
        CollectData();

        string json = JsonUtility.ToJson(CurrentSaveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Data Saved (Path : " + Application.persistentDataPath + "/savefile.json )");

        LogCurrentData();
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path) == false)
        {
            CreateAndSaveData();
        }

        string json = File.ReadAllText(path);
        CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Data Loaded (Path : " + Application.persistentDataPath + "/savefile.json )");

        LogCurrentData();

        OnSaveDataLoadedEvent?.Invoke(CurrentSaveData);
    }

    private void CreateAndSaveData()
    {
        CurrentSaveData = new SaveData();

        string json = JsonUtility.ToJson(CurrentSaveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("New Data Saved (Path : " + Application.persistentDataPath + "/savefile.json )");
    }

    private void LogCurrentData()
    {
        string LogString = "[UNLOCKED ANIMAL LIST] \n";
        for (int i = 0; i < CurrentSaveData.UnlockedAnimalList.Count; i++)
        {
            LogString += CurrentSaveData.UnlockedAnimalList[i] + "\n";
        }
        Debug.Log(LogString);

        LogString = "[SOUND SETTING] \n";
        LogString += "BGM Volume : ( " + CurrentSaveData.BGMVolume + " / 1 ) \n";
        LogString += "SFX Volume : ( " + CurrentSaveData.SFXVolume + " / 1 ) \n";
        Debug.Log(LogString);

        LogString = "[SCORE] \n";
        LogString += "High Score : " + CurrentSaveData.HighScore + "\n";
        Debug.Log(LogString);
    }

    private void CollectData()
    {
        // Unlocked Animal
        CurrentSaveData.UnlockedAnimalList = AnimalDataManager.Instance.UnlockedAnimalList;

        // Sound Setting
        CurrentSaveData.BGMVolume = SoundManager.Instance.SoundSettingData.BGMVolume;
        CurrentSaveData.SFXVolume = SoundManager.Instance.SoundSettingData.SFXVolume;

        // High Score
        CurrentSaveData.HighScore = GameManager.Instance.HighScore;
    }
}
