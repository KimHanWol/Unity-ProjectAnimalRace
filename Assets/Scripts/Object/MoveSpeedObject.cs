using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedObject : MonoBehaviour
{
    Rigidbody2D RigidBody2D;

    // Start is called before the first frame update
    void Start()
    {
        RigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < -100)
        {
            transform.position = Vector2.zero;
        }
    }

    public float GetCurrentVelocity()
    {
        return RigidBody2D.velocity.x;
    }
}
