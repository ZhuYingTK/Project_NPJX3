using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravity : MonoBehaviour
{

    public float gravityStrength = 10f; // 引力强度

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
        // 检测在触发器范围内的其他物体
        if (other.CompareTag("Cat") || other.CompareTag("Mouse")) // 假设玩家标签为"Player"
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            // 计算引力方向
            Vector2 direction = transform.position - other.transform.position;

            // 计算引力大小，根据距离的倒数增加引力
            //float distance = direction.magnitude;
            float gravityForce = gravityStrength;

            // 应用引力力
            rb.AddForce(direction.normalized * gravityForce);
        }
    }

}
