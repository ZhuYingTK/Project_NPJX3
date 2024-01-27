using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dust_collector : MonoBehaviour
{
    

    public float moveSpeed = 0.5f; // �ƶ��ٶ�
    public float changeDirectionInterval = 1f; // �ı䷽��ļ��ʱ��
    private Rigidbody2D rigidbody; // �������
    private float timer; // ��ʱ��
    private Vector3 randomDirection;

    void Start()
    {
        // ��ȡ�������
        rigidbody = GetComponent<Rigidbody2D>();
        // ��ʼ����ʱ��
        timer = changeDirectionInterval;
    }

    void Update()
    {
        // ���¼�ʱ��
        timer -= Time.deltaTime;

        // ����ʱ���ﵽ��ʱ��������������ƶ�����
        if (timer <= 0f)
        {
            GenerateRandomDirection();
            timer = changeDirectionInterval;
        }

        // ʩ������ʹ�ø����AddForce����
        rigidbody.AddForce(randomDirection * moveSpeed);
    }

    // ��������ƶ�����
    void GenerateRandomDirection()
    {
        // ����һ������Ķ�ά����
        Vector2 random2D = UnityEngine.Random.insideUnitCircle.normalized;
        // ת��Ϊ��ά����
        randomDirection = new Vector3(random2D.x, 0f, random2D.y);
    }



    



}