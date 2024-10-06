using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    // GameObject
    public AnimalDataManager AnimalDataManger;

    // Data
    [Header("Animation")]
    public float RunAnimationSpeedRate = 1;
    public float RunAnimationMaxSpeedRate = 5;

    static public GameManager Get()
    {
        return instance;
    }

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
}
