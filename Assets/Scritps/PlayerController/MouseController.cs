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

    public List<Transform> teleportPoints; // 传送点数组
    public Transform[] cheesePoints;
    public float teleportRadius = 2f; // 玩家必须在此半径内按下 "E" 键才能传送

    private int currentTeleportPointIndex = 0;
    private int timeLeft = 0;
    private bool onChoice = false;
    private bool isDead = false;
    private bool moveable = false;

    public static MouseController Instance;

    private bool isCheese = false;
    public float cheeseEatingRate = 1f;

    private float holeCD = 0f;

    private bool isSlip = false;
    private float SlipTimmer = 0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Instance = this;

        EventCenter.AddSingleEventListener(EventKey.HoleCreateDown, (List<Transform> holeTrans) => { teleportPoints = holeTrans; });
        EventCenter.AddEventListener(EventKey.GameStart, () => { moveable = true; });
        EventCenter.AddEventListener(EventKey.GameEnd, () => { moveable = false; });

        //Debug.Log("事件注册");
    }


    private void Update()
    {

        Vector3 currentVelocity = rb.velocity;
        float horizontalSpeed = rb.velocity.x;

        //Debug.Log(horizontalSpeed);

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
        /*
        if (isCheese)
        {
            Transform closeCheese = FindClosestCheese(transform);
            if(closeCheese == null)
            {
                Stop_cheese();
                Debug.Log("没有可以吃的cheese");
            }
            else
            {
                //TODO:挖矿逻辑
                Transform triangleTransform = closeCheese.GetChild(0);
                cheese remain_cheese = triangleTransform.GetComponent<cheese>();

                Debug.Log(remain_cheese.cheeseHP);

                remain_cheese.cheeseHP -= Time.deltaTime * cheeseEatingRate;

                if(remain_cheese.cheeseHP <= 0)
                {
                    Stop_cheese();
                    remain_cheese.cheeseHP = 0;
                    closeCheese.position = new Vector3(10000000f, 0f, 0f);
                }

            }
        }
        */

        if (moveable)
        {
            float inputX1 = Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            float inputX2 = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            float inputY1 = Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
            float inputY2 = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0);
            inputVector = new Vector2(inputX1 + inputX2, inputY1 + inputY2).normalized;
            //Debug.Log(inputVector);
        }

        Debug.Log("holeCD:" + holeCD);
        if (holeCD > 0)
        {
            holeCD -= Time.deltaTime;
        }
        else
        {
            if (onChoice)
            {
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    SelectCurrentOption();
                    Debug.Log("holeCD:" + holeCD);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    //Teleport();
                    
                    if (FindClosestTeleportPoint(transform) != null)
                    {
                        currentTeleportPointIndex = teleportPoints.IndexOf(FindClosestTeleportPoint(transform));
                        StartCoroutine(ChangeOptionEverySecond());
                    }

                    
                    //spriteRenderer.enabled = false;
                }
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Start_cheese();
        }

        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            Stop_cheese();
        }


        if (isSlip)
        {
            if (SlipTimmer <= 0)
            {
                isSlip = false;
                moveable = true;
            }
            else
            {
                SlipTimmer -= Time.deltaTime;
            }
        }


    }

    private void Start_cheese()
    {
        moveable = false;
        EventCenter.TriggerEvent(EventKey.MouseStartSteal);
    }

    private void Stop_cheese()
    {
        moveable = true;
        EventCenter.TriggerEvent(EventKey.MouseEndSteal);
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

    /*
    void Teleport()
    {
        // 检查传送点数组是否为空
        if (teleportPoints.Count == 0)
        {
            Debug.LogError("No teleport points defined!");
            return;
        }

        // 获取目标传送点的位置
        StartCoroutine(WaitForTwoSeconds());


    }
    */
    Transform FindClosestCheese(Transform transform)
    {
        foreach (Transform cheesePoint in cheesePoints)
        {
            if (Vector2.Distance(transform.position, cheesePoint.position) <= teleportRadius)
            {
                return cheesePoint;
            }
        }

        return null;
    }


    Transform FindClosestTeleportPoint(Transform currentPoint)
    {
        foreach (Transform teleportPoint in teleportPoints)
        {
            if (IsPlayerNearTeleportPoint(teleportPoint))
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


    /*
    IEnumerator WaitForTwoSeconds()
    {

        Transform targetTeleportPoint = teleportPoints[Random.Range(0, teleportPoints.Count)];

        // 查找玩家是否接近任何传送点，并传送
        Transform closeTeleportPoint = FindClosestTeleportPoint(transform);

        if (closeTeleportPoint != null)
        {
            // 传送玩家到目标传送点

            // 等待2秒钟

            //Debug.Log(targetTeleportPoint);

            change_color(targetTeleportPoint, Color.blue);

            yield return new WaitForSeconds(2.0f);

            change_color(targetTeleportPoint, Color.red);

            // 在这里添加你想要在等待结束后执行的代码
            //Debug.Log("等待结束，2秒过去了！");

            transform.position = targetTeleportPoint.position;
        }
        else
        {
            Debug.Log("Player is not close enough to any teleport point.");
        }
    }
    */



    System.Collections.IEnumerator ChangeOptionEverySecond()
    {
        onChoice = true;
        moveable = false;
        timeLeft = 3;
        spriteRenderer.enabled = false;
        animator.SetBool("inChoice", true);

        teleportPoints[currentTeleportPointIndex].gameObject.GetComponent<HoleBehavior>().SetToActive();

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
        teleportPoints[currentTeleportPointIndex].GetComponent<HoleBehavior>().SetToCommon();
        
        currentTeleportPointIndex = (currentTeleportPointIndex + 1) % teleportPoints.Count;

        teleportPoints[currentTeleportPointIndex].GetComponent<HoleBehavior>().SetToActive();

        Debug.Log("Current Option: " + teleportPoints[currentTeleportPointIndex]);
        timeLeft -= 1;
    }


    void SelectCurrentOption()
    {
        Debug.Log("Selected Option: " + teleportPoints[currentTeleportPointIndex]);

        onChoice = false;

        moveable = true;

        timeLeft = 0;

        holeCD = 5f;

        animator.SetBool("inChoice", false);

       teleportPoints[currentTeleportPointIndex].gameObject.GetComponent<HoleBehavior>().SetToCommon();

        transform.position = teleportPoints[currentTeleportPointIndex].position;

        spriteRenderer.enabled = true;
    }

    public void OnSlip(float slipTime)
    {
        isSlip = true;
        SlipTimmer = slipTime;
        moveable = false;
        rb.velocity = rb.velocity.normalized * maxSpeed;
        Debug.Log("sliping!!!");
    }


}