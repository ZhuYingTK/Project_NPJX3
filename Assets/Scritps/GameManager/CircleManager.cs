using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    public static CircleManager instance;


    [SerializeField,Label("��Ԥ����")]
    public GameObject circlePrefab; // Բ�ε�Ԥ����
    private GameObject previewCircle; // Ԥ����Բ��

    [SerializeField, Label("�Ƿ��ڴ���ģʽ")]
    public bool IsCreateMode = false;

    [SerializeField, Label("��󴴽�����")]
    public int MaxHoleCount = 3;

    [SerializeField, Label("��ǰ��������")]
    public int CurHoleCount = 0;
    [SerializeField,Label("��ǰ��")]
    List<GameObject> HoleList = new List<GameObject>();


    Vector2 LastMousePos;  //�����ɴ�����Χʱ���洢�ϴε�λ��

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
        // ���Ԥ��Բ�β����ڣ��ʹ���һ��
        if (previewCircle == null)
        {
            previewCircle = Instantiate(circlePrefab, Vector3.zero, Quaternion.identity);
            //previewCircle.SetActive(false); 
        }

        // ��ȡ���λ��
        Vector3 mousePosition = GetMousePosition();

        // ����Ԥ��Բ�ε�λ��
        previewCircle.transform.position = mousePosition;
    }

    void HandleMouseInput()
    {
        // ������������
        if (Input.GetMouseButtonDown(0))
        {
            // ��ȡ���λ��
            Vector3 mousePosition = GetMousePosition();

            // ����һ���µ�Բ��
            var hole = Instantiate(circlePrefab, mousePosition, Quaternion.identity);

            AddHole(hole);

        }
    }

    Vector3 GetMousePosition()
    {
        // ��ȡ�������Ļ�ϵ�λ��
        Vector3 mousePosition = Vector3.zero;

        // ���߼����ײ���Ƿ񱻵��
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
        // ��Ϊnull������Ϊ������ײ��
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
            // ��������ģʽ����ʼ��Ϸ
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
