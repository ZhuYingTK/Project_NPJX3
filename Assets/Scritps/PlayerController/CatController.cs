using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField,Label("加速度")]
    public float acceleration = 5.0f; // 加速度
    [SerializeField,Label("最大速度")]
    public float maxSpeed = 10.0f; // 最大速度
    [SerializeField,Label("减速度")]
    public float deceleration = 5.0f;// 减速度
    [SerializeField, Label("弹射速度")]
    public float shootingSpeed;
    [SerializeField, Label("弹射结束减速度")] 
    public float shootDeceleration = 10f;
    [SerializeField, Label("弹射距离")] 
    public float shootDistance = 10;
    [SerializeField, Label("弹射弹性")]
    public float bounciness = 1;
    [SerializeField,Label("箭头旋转速度")]
    public float rotationSpeed = 90;//旋转速度
    [SerializeField,Label("箭头指示物")]
    public GameObject Arrow;    //  箭头指示物
    
    public PhysicsMaterial2D shootPhyMaterial;
    public PhysicsMaterial2D originPhyMaterial;

    private Rigidbody2D rb;
    private Vector2 inputVector;
    private bool isPreShooting;     //弹射预备状态
    private bool isShooting;        //弹射状态
    private Vector2 shootVector;    //发射方向
    private float currentDistance;  //当前距离

    private Vector3 _lastPosition;  //上次的位置

    private void Start()
    {
        isPreShooting = false;
        rb = GetComponent<Rigidbody2D>();
        rb.sharedMaterial = originPhyMaterial;
    }

    private void Update()
    {
        //普通
        if (!isPreShooting && !isShooting)
        {
            float inputX1 = Input.GetKey(KeyCode.A) ? -1 : 0;
            float inputX2 = Input.GetKey(KeyCode.D) ? 1 : 0;
            float inputY1 = Input.GetKey(KeyCode.S) ? -1 : 0;
            float inputY2 = (Input.GetKey(KeyCode.W) ? 1 : 0);
            inputVector = new Vector2(inputX1 + inputX2, inputY1 + inputY2).normalized;
            if (Input.GetKeyUp(KeyCode.J))
            {
                ChangeToPreShooting();
            }
        }
        //预备弹射状态
        else if(isPreShooting)
        {
            inputVector = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                shootVector = RotateVector(shootVector, rotationSpeed * Time.deltaTime);
            }
            // 按下D键时顺时针旋转
            if (Input.GetKey(KeyCode.D))
            {
                shootVector = RotateVector(shootVector, -rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKeyUp(KeyCode.J))
            {
                Shooting();
            }
            Arrow.transform.rotation = Quaternion.Euler(0, 0, VectorToAngle(shootVector));
        }
        //弹射状态
        else if(isShooting)
        {
            //增加当前距离
            currentDistance += Vector2.Distance(transform.position, _lastPosition);
            if (currentDistance > shootDistance)
            {
                EndShooting();
            }
        }
        _lastPosition = transform.position;
    }

    //切换到弹射状态
    private void ChangeToPreShooting()
    {
        isPreShooting = true;
        Arrow.SetActive(true);
        shootVector = rb.velocity.normalized==Vector2.zero ? Vector2.right : rb.velocity.normalized;
    }

    //发射！
    private void Shooting()
    {
        isPreShooting = false;
        isShooting = true;
        Arrow.SetActive(false);
        //更改速度
        rb.velocity = shootingSpeed * shootVector;
        rb.sharedMaterial = shootPhyMaterial;
    }

    private void EndShooting()
    {
        currentDistance = 0;
        isShooting = false;
        rb.sharedMaterial = originPhyMaterial;
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

        // 限制最大速度,弹射状态不减速
        if (!isShooting)
        {
            // 限制最大速度
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity -= rb.velocity.normalized * shootDeceleration * Time.fixedDeltaTime;
            }
            // 如果没有输入，逐渐减速,弹射状态不减速
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
    
    
    // 旋转向量的函数
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);

        return v;
    }
    
    // 将向量转换为角度的函数
    private float VectorToAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
}