using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SaveData
{
    public SaveData()
    {
        SaveVersion = SaveSystem.CURRENT_SAVE_VERSION;

        UnlockedAnimalList = new List<AnimalType>();
        UnlockedAnimalList.Add(AnimalType.Dog);

        UnlockedFeverAnimalList = new List<AnimalType>();

        BGMVolume = 0.7f;
        SFXVolume = 0.7f;

        HighScore = 0;
    }

    //Save Data Version
    public int SaveVersion;

    //Unlocked Animal
    public List<AnimalType> UnlockedAnimalList;

    //Unlocked Fever Animal
    public List<AnimalType> UnlockedFeverAnimalList;

    //Sound Setting
    public float BGMVolume;
    public float SFXVolume;

    //Score
    public int HighScore;
}

public class SaveSystem : SingletonObject<SaveSystem>
{
    public static readonly int CURRENT_SAVE_VERSION = 10002;

    public static readonly int SAVE_VERSION_FIRST = 10001; // 초기 세이브 버전
    public static readonly int SAVE_VERSION_FEVER_ANIMAL = 10002; // 피버 타임 동물 언락 데이터 추가 버전

    private SaveData CurrentSaveData;

    public UnityEvent<SaveData> OnSaveDataLoadedEvent;

    // 다른 시스템에서 이벤트 관련 로직을 처리한 다음에
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

        //버전이 다르면 마이그레이트
        if (CurrentSaveData.SaveVersion != CURRENT_SAVE_VERSION)
        {
            MigrateData();
        }

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

    public void ResetData()
    {
        CreateAndSaveData();
        LogCurrentData();
    }

    private void MigrateData()
    {
        // 피버 타임 동물 언락 데이터
        if (CurrentSaveData.SaveVersion < SAVE_VERSION_FEVER_ANIMAL)
        {
            Debug.Log("Data Migrated ( Version : " + CurrentSaveData.SaveVersion + " -> " + SAVE_VERSION_FEVER_ANIMAL + " )");

            CurrentSaveData.SaveVersion = SAVE_VERSION_FEVER_ANIMAL;
            // 피버 타임 동물 언락 데이터는 기본값 사용하므로 따로 처리하지 않음
        }
    }

    private void LogCurrentData()
    {
        string LogString = "[UNLOCKED ANIMAL LIST] \n";
        for (int i = 0; i < CurrentSaveData.UnlockedAnimalList.Count; i++)
        {
            LogString += CurrentSaveData.UnlockedAnimalList[i] + "\n";
        }

        LogString += "[UNLOCKED FEVER ANIMAL LIST]\n";
        for (int i = 0; i < CurrentSaveData.UnlockedFeverAnimalList.Count; i++)
        {
            LogString += CurrentSaveData.UnlockedFeverAnimalList[i] + "\n";
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

        // Unlocked Animal
        CurrentSaveData.UnlockedFeverAnimalList = AnimalDataManager.Instance.UnlockedFeverAnimalList;

        // Sound Setting
        CurrentSaveData.BGMVolume = SoundManager.Instance.SoundSettingData.BGMVolume;
        CurrentSaveData.SFXVolume = SoundManager.Instance.SoundSettingData.SFXVolume;

        // High Score
        CurrentSaveData.HighScore = GameManager.Instance.HighScore;
    }
}
