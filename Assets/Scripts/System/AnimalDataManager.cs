using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InputType
{
    AD_TakeTurn, // AD 번갈아서 누르기
    QWER_TakeTurn, // QWER 번갈아서 누르기
    ArrowRightLeft_TakeTurn, // 화살표 좌우 번갈아서 누르기
    QWAS_IOKL_TakeTurn, // QWAS 동시입력, IOKL 동시입력 번갈아서 누르기
    MouseLeftRight_TakeTurn, // 마우스 좌우 번갈아서 누르기 

    QWERASDF, // QWERASDF 막 누르기
    ZXDotSlash, // ZX>? 막 누르기
    SpaceBar, // 스페이스바

    MouseScrollUp, // 마우스 스크롤 올리기
    MouseScrollDown, // 마우스 스크롤 내리기
    MouseVerticalHorizonal, // 마우스 수직 수평
    Mouse8, // 마우스 8자로 그리기

    SpaceBarToCrash, // 타이밍 맞춰서 스페이스 바
}

[System.Serializable]
public class InputData
{
    public InputType InputType;
    public int InputStackCount; //몇 번 눌러야 한 사이클인지 (AD 번갈아 누를 땐 2, QWER 번갈아 누를 땐 4)
    public float Veclocity;
    public AnimationClip KeyGuideAnimation;
}

[System.Serializable]
public class AnimalData
{
    public AnimalType AnimalType;
    public InputType InputType;
    public RuntimeAnimatorController Animator;
}

public class AnimalDataManager : SingletonObject<AnimalDataManager>
{
    public InputData[] InputDataList;
    public AnimalData[] AnimalDataList;
    public List<AnimalType> UnlockedAnimalList;

    protected override void Awake()
    {
        base.Awake();

        SaveSystem.Instance.OnSaveDataLoadedEvent.AddListener(OnSaveDataLoaded);
        EventManager.Instance.OnNewAnimalUnlockStartEvent.AddListener(OnNewAnimalUnlocked);
    }

    private void OnSaveDataLoaded(SaveData LoadedSaveData)
    {
        UnlockedAnimalList = LoadedSaveData.UnlockedAnimalList;
    }

    private void OnNewAnimalUnlocked(AnimalType UnlockedAnimalType)
    {
        UnlockedAnimalList.Add(UnlockedAnimalType);
        string LogString = "";
        LogString += "New animal unlocked : " + UnlockedAnimalType.ToString() + "\n";
        LogString += "Current unlocked animal count ( " + UnlockedAnimalList.Count + " / " + AnimalDataList.Length + " )\n";
        Debug.Log(LogString);
        SaveSystem.Instance.SaveData();
    }

    public AnimalData GetAnimalData(AnimalType TargetAnimalType)
    {
        foreach (AnimalData InAnimalData in AnimalDataList)
        {
            if (InAnimalData.AnimalType == TargetAnimalType)
            {
                return InAnimalData;
            }
        }
        return null;
    }

    public AnimalData GetUnlockedAnimalDataByRandom(AnimalType CurrentAnimalType)
    {
        int TargetAnimalDataIndex = 0;
        do
        {
            TargetAnimalDataIndex = Random.Range(0, UnlockedAnimalList.Count);
        }
        while (UnlockedAnimalList[TargetAnimalDataIndex] == CurrentAnimalType);

        return GetAnimalData(UnlockedAnimalList[TargetAnimalDataIndex]);
    }

    public AnimalData GetLockedAnimalDataByRandom()
    {
        int LockedAnimalCount = AnimalDataList.Length - UnlockedAnimalList.Count;
        int TargetAnimalDataIndex = Random.Range(0, LockedAnimalCount);
        foreach (AnimalData AnimalData in AnimalDataList)
        {
            if (UnlockedAnimalList.Contains(AnimalData.AnimalType) == true)
            {
                continue;
            }

            if (TargetAnimalDataIndex == 0)
            {
                return AnimalData;
            }
            TargetAnimalDataIndex--;
        }

        return null;
    }

    private int GetAnimalIndex(AnimalType TargetAnimalType)
    {
        for (int i = 0; i < AnimalDataList.Length; i++)
        {
            AnimalData InAnimalData = AnimalDataList[i];
            if (InAnimalData.AnimalType == TargetAnimalType)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetInputStackCount(InputType InInputType)
    {
        foreach (InputData InInputData in InputDataList)
        {
            if (InInputData.InputType == InInputType)
            {
                return InInputData.InputStackCount;
            }
        }
        return -1;
    }

    public float GetVelocity(InputType InInputType)
    {
        foreach (InputData InInputData in InputDataList)
        {
            if (InInputData.InputType == InInputType)
            {
                return InInputData.Veclocity;
            }
        }
        return 0;
    }

    public AnimationClip GetInputTypeAnimationClip(InputType InInputType)
    {
        foreach (InputData InInputData in InputDataList)
        {
            if (InInputData.InputType == InInputType)
            {
                return InInputData.KeyGuideAnimation;
            }
        }
        return null;
    }
}
