using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameObject
    public PlayerController Player;
    public MapManager MapManager;

    void Start()
    {
    }

    public void Update_CheckSpeed()
    {
        if (Player == null)
        {
            return;
        }

        float CurrentVelocity = Player.GetVelocity();

        if (MapManager != null)
        {
            MapManager.UpdateSpeed(CurrentVelocity);
        }
    }

    void Update()
    {
        Update_CheckSpeed();
    }
}
