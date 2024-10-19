using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class HunterController : MonoBehaviour
{
    public float HunterMovementRate = 20f;

    [Header("Force")]
    public float ForceInitial;
    public float ForceIncreaseRate;

    [Header("Delay")]
    public float FirstDelay = 1f;
    public float DelayInitial = 1f;
    public float DelayDecreaseRate = 0.01f;
    public float DelayMin = 0.05f;

    private float CurrentForceRate = 1f;
    private float CurrentDurationRate = 1f;

    private Rigidbody2D RigidBody2D;

    // Start is called before the first frame update
    void Start()
    {
        RigidBody2D = GetComponent<Rigidbody2D>();

        StartMovement();
    }

    private void StartMovement()
    {
        StartCoroutine(WaitFirstDelay());
    }

    private IEnumerator WaitFirstDelay()
    {
        yield return new WaitForSeconds(FirstDelay);
        StartCoroutine(MoveHunter());
    }

    private IEnumerator MoveHunter()
    {
        float CurrentDuration = Mathf.Clamp(DelayInitial * CurrentDurationRate, DelayMin, DelayInitial);

        Internal_MoveHunter();
        yield return new WaitForSeconds(CurrentDuration);

        CurrentDurationRate -= DelayDecreaseRate;
        CurrentForceRate += ForceIncreaseRate;

        StartCoroutine(MoveHunter());
    }

    private void Internal_MoveHunter()
    {
        float CurrentForce = ForceInitial * CurrentForceRate;
        
        if(RigidBody2D != null)
        {
            RigidBody2D.AddForce(new Vector2(CurrentForce, 0));
        }
    }

    public void OnPlayerAccelerated(float MoveForce)
    {
        RigidBody2D.AddForce(new Vector2(-MoveForce * HunterMovementRate, 0));
    }
}
