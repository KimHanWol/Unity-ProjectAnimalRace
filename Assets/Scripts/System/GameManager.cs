using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    // GameObject
    public PlayerController Player;
    public AnimalDataManager AnimalDataManger;
    public MapManager MapManager;

    // Data
    [Header("Movement")]
    public float RunVelocityRate = 10;

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
            // 다른 GameManager 가 이미 생성된 경우 삭제
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Player.OnPlayerAccelerated.AddListener(Call_OnPlayerAccelerated);
    }

    private void Call_OnPlayerAccelerated(float Velocity)
    {
        MapManager.OnPlayerAccelerated(Velocity);
    }
}
