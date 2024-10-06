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
            // �ٸ� GameManager �� �̹� ������ ��� ����
            Destroy(gameObject);
            return;
        }
    }

    static public GameManager Get()
    {
        return instance;
    }
}
