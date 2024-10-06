using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public AnimalDataManager AnimalDataManger;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // 다른 GameManager 가 이미 생성된 경우 삭제
            Destroy(gameObject);
            return;
        }
    }

    static public GameManager Get()
    {
        return instance;
    }
}
