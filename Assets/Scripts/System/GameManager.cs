using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameObject
    public PlayerController Player;
    public MapManager MapManager;

    public RuningObject[] SpawnedObjectList;

    void Start()
    {
        Player.OnPlayerMovementEnableChanged.AddListener(OnAnimalChangeEffectStateChanged);
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

    private void OnAnimalChangeEffectStateChanged(bool Enabled)
    {
        // Object Spawner
        foreach(RuningObject SpawnedObject in SpawnedObjectList)
        {
            SpawnedObject.EnableMovement(Enabled);
        }

        // Map Manager
        if(MapManager != null)
        {
            MapManager.EnableMovement(Enabled);
        }
    }
}
