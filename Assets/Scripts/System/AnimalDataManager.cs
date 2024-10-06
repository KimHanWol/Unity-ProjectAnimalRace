using System;
using UnityEditor.Animations;
using UnityEngine;

public enum InputType
{
    AD_TakeTurn, // AD 번갈아서 누르기
    QWER_TakeTurn, // QWER 번갈아서 누르기
    SpaceBar, // 스페이스바
    MouseScrollUp, // 마우스 스크롤 올리기
    MouseScrollDown, // 마우스 스크롤 내리기
    MouseVerticalHorizonal, // 마우스 수직 수평
    Mouse8, // 마우스 8자로 그리기
    SpaceBarToCrash, // 타이밍 맞춰서 스페이스 바
    ArrowRightLeft, // 화살표 좌우
}

[System.Serializable]
public class InputData
{
    public InputType InputType;
    public int InputStackCount; //몇 번 눌러야 한 사이클인지 (AD 번갈아 누를 땐 2, QWER 번갈아 누를 땐 4)
    public int Veclocity;
}

[System.Serializable]
public class AnimalData
{
    public AnimalType AnimalType;
    public InputData InputData;
    public AnimatorController Animator;
}

public class AnimalDataManager : MonoBehaviour
{
    public AnimalData[] AnimalDataList;

    public AnimalData GetAnimalData(AnimalType TargetAnimalType)
    {
        foreach(AnimalData InAnimalData in AnimalDataList)
        {
            if(InAnimalData.AnimalType == TargetAnimalType)
            {
                return InAnimalData;
            }
        }
        return null;
    }
}