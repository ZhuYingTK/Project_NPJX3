using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravity : MonoBehaviour
{

    public float gravityStrength = 10f; // ����ǿ��

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerStay2D(Collider2D other)
    {
        // ����ڴ�������Χ�ڵ���������
        if (other.CompareTag("Cat") || other.CompareTag("Mouse")) // ������ұ�ǩΪ"Player"
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            // ������������
            Vector2 direction = transform.position - other.transform.position;

            // ����������С�����ݾ���ĵ�����������
            //float distance = direction.magnitude;
            float gravityForce = gravityStrength;

            // Ӧ��������
            rb.AddForce(direction.normalized * gravityForce);
        }
    }

}