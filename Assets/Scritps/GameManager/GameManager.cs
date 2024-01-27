using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("��ɫ���")]
    // è�Ƿ�׼����
    [SerializeField, Label("è�Ƿ�׼����")]
    public bool IsCatReady = false;
    [Label("èԤ����")]
    public GameObject CatPrefab;
    GameObject cat;
    //�����Ƿ�׼����
    [SerializeField, Label("�����Ƿ�׼����")]
    public bool IsMouseReady = false;
    [Label("����Ԥ����")]
    public GameObject MousePrefab;
    GameObject mouse;


    [Header("׼���׶����")]
    [SerializeField, Label("�Ƿ���׼���׶�")]
    public bool IsPreparePeriod = false;


    [Header("׷��׶����")]
    [SerializeField, Label("�Ƿ���׷��׶�")]
    public bool IsChasingPeriod = false;
    [SerializeField, Label("׷��׶ο�ʼ����ʱ")]
    public float ChasingPeriodStartCountDown = 5f;
    [SerializeField, Label("׷��׶�ʱ������")]
    float LevelTime = 60f;

    [Label("�����Ŀ��ʳ�����")]
    public int MouseNeedFoodCount = 3;
    [Label("�����ѻ�õ�ʳ�����")]
    public int MouseGetFoodCount = 0;
    [Label("�����Ƿ�ץ��")]
    public bool IsMouseCatched = false;

    //��ʼ����ʱʣ��ʱ��
    [HideInInspector]
    public float RemainStartLevelTime;
    //ʣ��ʱ��
    [HideInInspector]
    public float RemainLevelTime;



    [Header("��Ϸ�ܽ׶����")]
    [SerializeField, Label("��Ϸ�Ƿ�ʼ")]
    public bool IsGaming = false;
    [Label("�淨����UI")]
    public GameObject IntroductionUI;
    [Label("����UI - ����Ӯ")]
    public GameObject MouseWinUI;
    [Label("����UI - èӮ")]
    public GameObject CatWinUI;

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

    private void Start()
    {
        EventCenter.AddSingleEventListener(EventKey.CatCatchMouse, () => { IsMouseCatched = true; });
        EventCenter.AddSingleEventListener(EventKey.MouseGetFood, () => { MouseGetFoodCount++; });
        GameReset();
    }

    private void Update()
    {
        
    }

    public void GameReset()
    {
        //��λ��ɫ
        if(cat == null && CatPrefab != null)
        {
            cat = Instantiate(CatPrefab, Vector3.zero, Quaternion.identity);
        }

        if (mouse == null && MousePrefab != null)
        {
            mouse = Instantiate(MousePrefab, Vector3.zero, Quaternion.identity);
        }

        //���ø�����
        RemainStartLevelTime = ChasingPeriodStartCountDown;
        RemainLevelTime = LevelTime;
        IsPreparePeriod = false;
        IsChasingPeriod = false;
        IsMouseCatched = false;
        MouseGetFoodCount = 0;
        // ����UI
        if (CatWinUI != null)
        {
            CatWinUI.SetActive(false);
        }
        if (MouseWinUI != null)
        {
            MouseWinUI.SetActive(false);
        }
        //�ؿ��ڸ���Ʒ״̬����
        //TODO:�������
        CircleManager.instance.ResetAllHole();
        //TODO:ʳ��״̬��λ

        //�����ܽ���
        if (IntroductionUI != null)
        {
            IntroductionUI.SetActive(true);
        }
    }

    public void GameStart()
    {
        if (IsGaming) return;
        if (IntroductionUI != null)
        {
            IntroductionUI.SetActive(false);
        }
        IsGaming = true;
        PreparePeriodStart();
    }

    public void PreparePeriodStart()
    {
        IsPreparePeriod = true;
        CircleManager.instance.StartCreateMode();
    }

    public void PreparePeriodEnd()
    {
        IsPreparePeriod = false;
        ChasingPeriodStart();
    }

    void ChasingPeriodStart()
    {
        IsChasingPeriod = true;
        // �������ݷ���
        EventCenter.TriggerEvent(EventKey.HoleCreateDown, CircleManager.instance.GetHoles());
        StartCoroutine(LevelTimeCountDown());
        IEnumerator LevelTimeCountDown()
        {
            while(RemainStartLevelTime > 0.0f)
            {
                RemainStartLevelTime--;
                yield return new WaitForSeconds(1.0f);
            }
            // TODO: ����ҿ�����
            EventCenter.TriggerEvent(EventKey.GameStart);
            while (RemainLevelTime > 0.0f && MouseGetFoodCount < MouseNeedFoodCount && !IsMouseCatched)
            {
                RemainLevelTime--;
                yield return new WaitForSeconds(1.0f);
            }
            IsChasingPeriod = false;
            // TODO: ���¼�������ҿ�����
            EventCenter.TriggerEvent(EventKey.GameEnd);
            // TODO: ���������
            GameEndAndConclusion();
        }
    }

    void GameEndAndConclusion()
    {
        IsGaming = false;
        if(IsMouseCatched || MouseNeedFoodCount > MouseGetFoodCount )
        {
            // èӮ
            if(CatWinUI != null)
            {
                CatWinUI.SetActive(true);
            }
        }
        else
        {
            //����Ӯ
            if (MouseWinUI != null)
            {
                MouseWinUI.SetActive(true);
            }
        }

        //if(mouse != null)
        //{
        //    Destroy(mouse);
        //}

        //if(cat != null)
        //{
        //    Destroy(cat);
        //}
    }



    [ContextMenu("testStart")]
    public void TestGameStart()
    {
        if (IsGaming) return;
        IsGaming = true;
        GameReset();
        PreparePeriodStart();
    }



}