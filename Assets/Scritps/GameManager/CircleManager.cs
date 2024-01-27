using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    public static CircleManager instance;


    [SerializeField,Label("洞预制体")]
    public GameObject circlePrefab; // 圆形的预制体
    private GameObject previewCircle; // 预览的圆形

    [SerializeField, Label("是否在创建模式")]
    public bool IsCreateMode = false;

    [SerializeField, Label("最大创建个数")]
    public int MaxHoleCount = 3;

    [SerializeField, Label("当前创建个数")]
    public int CurHoleCount = 0;
    [SerializeField,Label("当前洞")]
    List<GameObject> HoleList = new List<GameObject>();


    Vector2 LastMousePos;  //超出可创建范围时鼠标存储上次的位置

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void Update()
    {
        if(IsCreateMode)
        {
            UpdatePreviewCircle();
            HandleMouseInput();
        }
        else 
        {
            if (previewCircle != null) 
            { 
                Destroy(previewCircle); 
            }
        }

    }

    void UpdatePreviewCircle()
    {
        // 如果预览圆形不存在，就创建一个
        if (previewCircle == null)
        {
            previewCircle = Instantiate(circlePrefab, Vector3.zero, Quaternion.identity);
            //previewCircle.SetActive(false); 
        }

        // 获取鼠标位置
        Vector3 mousePosition = GetMousePosition();

        // 更新预览圆形的位置
        previewCircle.transform.position = mousePosition;
    }

    void HandleMouseInput()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标位置
            Vector3 mousePosition = GetMousePosition();

            // 创建一个新的圆形
            var hole = Instantiate(circlePrefab, mousePosition, Quaternion.identity);

            AddHole(hole);

        }
    }

    Vector3 GetMousePosition()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePosition = Vector3.zero;

        // 射线检测碰撞器是否被点击
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
        // 不为null，则认为有物体撞到
        if (hit.collider != null)
        {
            var hitObj = hit.collider.gameObject;
            if (hitObj.tag == "RoomGround")
            {
                mousePosition = hit.point;
                LastMousePos = mousePosition;
            }
            else
            {
                mousePosition = LastMousePos;
            }
        }
        else
        {
            mousePosition = LastMousePos;
        }
        return mousePosition;
    }

    [ContextMenu("CreateModeOn")]
    public void StartCreateMode()
    {
        ResetAllHole();
        IsCreateMode = true;
    }

    public void EndCreateMode()
    {
        IsCreateMode = false;
        GameManager.instance.PreparePeriodEnd();
    }

    void AddHole(GameObject hole)
    {
        HoleList.Add(hole);
        CurHoleCount++;
        if(CurHoleCount >= MaxHoleCount)
        {
            // 结束创建模式并开始游戏
            EndCreateMode();
        }
    }

    public void ResetAllHole()
    {
        foreach (var hole in HoleList) { Destroy(hole); }
        HoleList.Clear();
        CurHoleCount = 0;
    }

    public List<Transform> GetHoles()
    {
        List<Transform> holeTrans = new List<Transform>();

        foreach(var hole in HoleList)
        {
            holeTrans.Add(hole.transform);
        }

        return holeTrans;
    }

}

