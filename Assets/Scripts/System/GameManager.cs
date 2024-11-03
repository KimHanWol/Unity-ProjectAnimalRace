using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GameManager : MonoBehaviour
{
    // GameObject
    public PlayerController Player;
    public HunterController Hunter;
    public ObjectSpawner ObjectSpawner;
    public MapManager MapManager;
    public UIManager UIManager;
    
    public RuningObject[] SpawnedObjectList;
    public GameOverObject GameOverObject;

    private bool IsPlaying = false;

    void Start()
    {
        Player.OnPlayerMovementEnableChanged.AddListener(OnAnimalChangeEffectStateChanged);
        Player.OnPlayerAccelerated.AddListener(OnPlayerAccelerated);
        GameOverObject.OnGameOver.AddListener(OnGameOver);
    }

    void Update()
    {
        Update_CheckSpeed();
        Update_WaitForAnyButtonPressed();
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

    private void Update_WaitForAnyButtonPressed()
    {
        if(IsPlaying == true)
        { 
            return;
        }

        if(Input.anyKeyDown == true)
        {
            OnStart();
        }
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

    private void OnPlayerAccelerated(float MoveForce)
    {
        if(Hunter != null)
        {
            Hunter.OnPlayerAccelerated(MoveForce);
        }
    }

    private void OnStart()
    {
        IsPlaying = true;

        if (UIManager != null)
        {
            UIManager.OnAnyButtonPressed();
        }

        if (Player != null)
        {
            Player.SetMoveEnabled(true);
        }

        if (Hunter != null)
        {
            Hunter.StartMovement();
        }

        if(ObjectSpawner != null)
        {
            ObjectSpawner.EnableSpawn(true);
        }
    }

    private void OnGameOver()
    {
        IsPlaying = false;

        if(Player != null)
        {
            Player.ResetPlayer();
        }

        if(Hunter != null)
        {
            Hunter.ResetHunter();
        }

        if(UIManager != null)
        {
            UIManager.PlayTitleFadeAnimation(true);
        }

        if (ObjectSpawner != null)
        {
            ObjectSpawner.EnableSpawn(false);
        }
    }
}
