using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

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
    private Vector2 StartPosition = Vector2.zero;

    private Rigidbody2D RigidBody2D;
    private Animator Animator;

    void Start()
    {
        StartPosition = transform.position;

        RigidBody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    public void ResetHunter()
    {
        Rigidbody2D PlayerRigidbody = GetComponent<Rigidbody2D>();
        if (PlayerRigidbody != null)
        {
            PlayerRigidbody.velocity = Vector2.zero;
        }

        CurrentForceRate = 1f;
        CurrentDurationRate = 1f;

        transform.position = StartPosition;

        StopAllCoroutines();
    }

    public void StartMovement()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player")
        {
            return;
        }

        Animator.SetBool("CanHit", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }

        Animator.SetBool("CanHit", false);
    }
}
