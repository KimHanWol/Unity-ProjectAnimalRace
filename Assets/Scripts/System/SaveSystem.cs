using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SaveData
{
    public SaveData()
    {
        UnlockedAnimalKeyList = new List<string>();
        UnlockedAnimalKeyList.Add("Dog");

        BGMVolume = 0.7f;
        SFXVolume = 0.7f;
    }

    //Unlocked Animal
    public List<string> UnlockedAnimalKeyList;

    //Sound Setting
    public float BGMVolume;
    public float SFXVolume;
}

public class SaveSystem : SingletonObject<SaveSystem>
{
    private SaveData CurrentSaveData;

    public UnityEvent<SaveData> OnSaveDataLoadedEvent;

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
        for (int i = 0; i < CurrentSaveData.UnlockedAnimalKeyList.Count; i++)
        {
            LogString += CurrentSaveData.UnlockedAnimalKeyList[i] + "\n";
        }
        Debug.Log(LogString);

        LogString = "[SOUND SETTING] \n";
        LogString += "BGM Volume : ( " + CurrentSaveData.BGMVolume + " / 1 ) \n";
        LogString += "SFX Volume : ( " + CurrentSaveData.SFXVolume + " / 1 ) \n";
        Debug.Log(LogString);
    }

    private void CollectData()
    {
        // Unlocked Animal
        CurrentSaveData.UnlockedAnimalKeyList = AnimalDataManager.Instance.UnlockedAnimalList;

        // Sound Setting
        CurrentSaveData.BGMVolume = SoundManager.Instance.SoundSettingData.BGMVolume;
        CurrentSaveData.SFXVolume = SoundManager.Instance.SoundSettingData.SFXVolume;
    }
}
