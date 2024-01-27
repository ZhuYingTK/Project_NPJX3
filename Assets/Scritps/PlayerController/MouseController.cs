using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MouseController : MonoBehaviour
{
    public float acceleration = 5.0f; // 加速度
    public float maxSpeed = 10.0f; // 最大速度
    public float deceleration = 5.0f;// 减速度
    public float bounceForce = 100f;//弹回的力

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 inputVector;

    public Transform[] teleportPoints; // 传送点数组
    public float teleportRadius = 2f; // 玩家必须在此半径内按下 "E" 键才能传送

    private int currentTeleportPointIndex = 0;
    private int timeLeft = 0;
    private bool onChoice = false;
    private bool isDead = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        Vector3 currentVelocity = rb.velocity;
        float horizontalSpeed = rb.velocity.x;

        Debug.Log(horizontalSpeed);

        animator.SetBool("isDead", isDead);

        if (currentVelocity.magnitude == 0f)
        {
            animator.SetBool("isStand", true);
        }
        else
        {
            animator.SetFloat("Speed", horizontalSpeed);
            animator.SetBool("isStand", false);
            if (horizontalSpeed >= 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }

        if (onChoice)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectCurrentOption();
            }
        }
        else
        {
            float inputX1 = Input.GetKey(KeyCode.A) ? -1 : 0;
            float inputX2 = Input.GetKey(KeyCode.D) ? 1 : 0;
            float inputY1 = Input.GetKey(KeyCode.S) ? -1 : 0;
            float inputY2 = (Input.GetKey(KeyCode.W) ? 1 : 0);
            inputVector = new Vector2(inputX1 + inputX2, inputY1 + inputY2).normalized;
            Debug.Log(inputVector);

            if (Input.GetKeyDown(KeyCode.E))
            {

                //Teleport();
                StartCoroutine(ChangeOptionEverySecond());
                //spriteRenderer.enabled = false;

            }
        }
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

    void OnCollisionEnter2D(Collision2D collision)
    {

        print('1');

        // 检查是否与其他对象发生碰撞
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 获取碰撞法线，用于计算反弹方向
            Vector2 normal = collision.contacts[0].normal;

            // 计算反弹方向
            Vector2 bounceDirection = Vector2.Reflect(transform.position.normalized, normal).normalized;

            // 使用AddForce施加反弹力
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
    }


    void Teleport()
    {
        // 检查传送点数组是否为空
        if (teleportPoints.Length == 0)
        {
            Debug.LogError("No teleport points defined!");
            return;
        }

        // 获取目标传送点的位置
        StartCoroutine(WaitForTwoSeconds());


    }


    Transform FindClosestTeleportPoint(Transform currentPoint)
    {
        foreach (Transform teleportPoint in teleportPoints)
        {
            if (teleportPoint != currentPoint && IsPlayerNearTeleportPoint(teleportPoint))
            {
                return teleportPoint;
            }
        }

        return null;
    }

    bool IsPlayerNearTeleportPoint(Transform teleportPoint)
    {
        return Vector2.Distance(transform.position, teleportPoint.position) <= teleportRadius;
    }



    IEnumerator WaitForTwoSeconds()
    {

        Transform targetTeleportPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];

        // 查找玩家是否接近任何传送点，并传送
        Transform closeTeleportPoint = FindClosestTeleportPoint(transform);

        if (closeTeleportPoint != null)
        {
            // 传送玩家到目标传送点

            // 等待2秒钟

            Debug.Log(targetTeleportPoint);

            change_color(targetTeleportPoint, Color.blue);

            yield return new WaitForSeconds(2.0f);

            change_color(targetTeleportPoint, Color.red);

            // 在这里添加你想要在等待结束后执行的代码
            Debug.Log("等待结束，2秒过去了！");

            transform.position = targetTeleportPoint.position;
        }
        else
        {
            Debug.Log("Player is not close enough to any teleport point.");
        }
    }



    void change_color(Transform target_transform, Color newColor)
    {

        Transform circleTransform = target_transform.GetChild(0);
        if (circleTransform != null)
        {
            // 获取子物体的Renderer组件
            Renderer childRenderer = circleTransform.GetComponent<Renderer>();

            // 检查是否找到了Renderer组件
            if (childRenderer != null)
            {
                // 获取材质数组
                Material[] materials = childRenderer.materials;

                // 遍历每个材质，修改颜色
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].color = newColor;
                }
            }
            else
            {
                Debug.LogError("Child object does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("Child object with the name 'Circle' not found.");
        }

    }

    System.Collections.IEnumerator ChangeOptionEverySecond()
    {
        onChoice = true;
        timeLeft = 3;
        spriteRenderer.enabled = false;

        change_color(teleportPoints[currentTeleportPointIndex], Color.blue);

        while (timeLeft > 0)
        {
            // 切换选项
            ChangeOption();

            // 等待1秒
            yield return new WaitForSeconds(1f);
        }

        if (onChoice)
        {
            SelectCurrentOption();
        }



    }


    void ChangeOption()
    {
        // 切换到下一个选项
        change_color(teleportPoints[currentTeleportPointIndex], Color.red);

        currentTeleportPointIndex = (currentTeleportPointIndex + 1) % teleportPoints.Length;

        change_color(teleportPoints[currentTeleportPointIndex], Color.blue);

        Debug.Log("Current Option: " + teleportPoints[currentTeleportPointIndex]);
        timeLeft -= 1;
    }


    void SelectCurrentOption()
    {
        Debug.Log("Selected Option: " + teleportPoints[currentTeleportPointIndex]);

        onChoice = false;

        timeLeft = 0;

        change_color(teleportPoints[currentTeleportPointIndex], Color.red);

        transform.position = teleportPoints[currentTeleportPointIndex].position;

        spriteRenderer.enabled = true;
    }








}