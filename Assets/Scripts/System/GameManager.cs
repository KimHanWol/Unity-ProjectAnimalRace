using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameObject
    public PlayerController Player;
    public MapManager MapManager;
    public Rigidbody2D MoveSpeedObject;

    void Start()
    {
        if(Player != null)
        {
            Player.OnPlayerAccelerated.AddListener(Call_OnPlayerAccelerated);
        }
    }

    public void Update_CheckSpeed()
    {
        float CurrentVelocity = MoveSpeedObject.velocity.x;

        if (MoveSpeedObject != null)
        {
            MapManager.UpdateSpeed(CurrentVelocity);
        }

        if(Player != null)
        {
            Player.ChangeVelocity(CurrentVelocity);
        }
    }

    private void Call_OnPlayerAccelerated(float Velocity)
    {
        MoveSpeedObject.AddForce(new Vector2(Velocity, 0));
    }

    void Update()
    {
        Update_CheckSpeed();
    }
}
