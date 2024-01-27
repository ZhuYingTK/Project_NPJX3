using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dust_collector : MonoBehaviour
{
    

    public float moveSpeed = 0.5f; // 移动速度
    public float changeDirectionInterval = 1f; // 改变方向的间隔时间
    private Rigidbody2D rigidbody; // 刚体组件
    private float timer; // 计时器
    private Vector3 randomDirection;

    void Start()
    {
        // 获取刚体组件
        rigidbody = GetComponent<Rigidbody2D>();
        // 初始化计时器
        timer = changeDirectionInterval;
    }

    void Update()
    {
        // 更新计时器
        timer -= Time.deltaTime;

        // 当计时器达到零时，重新生成随机移动方向
        if (timer <= 0f)
        {
            GenerateRandomDirection();
            timer = changeDirectionInterval;
        }

        // 施加力，使用刚体的AddForce方法
        rigidbody.AddForce(randomDirection * moveSpeed);
    }

    // 生成随机移动方向
    void GenerateRandomDirection()
    {
        // 生成一个随机的二维向量
        Vector2 random2D = UnityEngine.Random.insideUnitCircle.normalized;
        // 转换为三维向量
        randomDirection = new Vector3(random2D.x, 0f, random2D.y);
    }



    



}
