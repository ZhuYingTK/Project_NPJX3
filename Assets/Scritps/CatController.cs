using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CataController : MonoBehaviour
{
    public float acceleration = 5.0f; // 加速度
    public float maxSpeed = 10.0f; // 最大速度
    public float deceleration = 5.0f;// 减速度

    private Rigidbody2D rb;
    private Vector2 inputVector;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float inputX1 = Input.GetKey(KeyCode.A) ? -1 : 0;
        float inputX2 = Input.GetKey(KeyCode.D) ? 1 : 0;
        float inputY1 = Input.GetKey(KeyCode.S) ? -1 : 0;
        float inputY2 = (Input.GetKey(KeyCode.W) ? 1 : 0);
        inputVector = new Vector2(inputX1 + inputX2, inputY1 + inputY2).normalized;
        Debug.Log(inputVector);
    }

    private void FixedUpdate()
    {
        // 是否有输入
        bool isInput = inputVector.x != 0 || inputVector.y != 0;

        // 如果有输入，应用加速度
        if (isInput)
        {
            rb.AddForce(inputVector * acceleration);
        }

        // 限制最大速度
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // 如果没有输入，逐渐减速
        if (!isInput && rb.velocity.magnitude > 0)
        {
            rb.velocity -= rb.velocity.normalized * deceleration * Time.fixedDeltaTime;
            if (rb.velocity.magnitude < 0.1f) // 速度很低时直接停止
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
}