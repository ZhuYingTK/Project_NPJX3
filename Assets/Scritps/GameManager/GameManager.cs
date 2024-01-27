using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("角色相关")]
    // 猫是否准备好
    [SerializeField, Label("猫是否准备好")]
    public bool IsCatReady = false;
    [Label("猫预制体")]
    public GameObject CatPrefab;
    GameObject cat;
    //老鼠是否准备好
    [SerializeField, Label("老鼠是否准备好")]
    public bool IsMouseReady = false;
    [Label("老鼠预制体")]
    public GameObject MousePrefab;
    GameObject mouse;


    [Header("准备阶段相关")]
    [SerializeField, Label("是否在准备阶段")]
    public bool IsPreparePeriod = false;


    [Header("追逐阶段相关")]
    [SerializeField, Label("是否在追逐阶段")]
    public bool IsChasingPeriod = false;
    [SerializeField, Label("追逐阶段开始倒计时")]
    public float ChasingPeriodStartCountDown = 5f;
    [SerializeField, Label("追逐阶段时长设置")]
    float LevelTime = 60f;

    [Label("老鼠的目标食物个数")]
    public int MouseNeedFoodCount = 3;
    [Label("老鼠已获得的食物个数")]
    public int MouseGetFoodCount = 0;
    [Label("老鼠是否被抓到")]
    public bool IsMouseCatched = false;

    //开始倒计时剩余时间
    [HideInInspector]
    public float RemainStartLevelTime;
    //剩余时间
    [HideInInspector]
    public float RemainLevelTime;



    [Header("游戏总阶段相关")]
    [SerializeField, Label("游戏是否开始")]
    public bool IsGaming = false;
    [Label("玩法介绍UI")]
    public GameObject IntroductionUI;
    [Label("结算UI - 老鼠赢")]
    public GameObject MouseWinUI;
    [Label("结算UI - 猫赢")]
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
        //复位角色
        if(cat == null && CatPrefab != null)
        {
            cat = Instantiate(CatPrefab, Vector3.zero, Quaternion.identity);
        }

        if (mouse == null && MousePrefab != null)
        {
            mouse = Instantiate(MousePrefab, Vector3.zero, Quaternion.identity);
        }

        //重置各参数
        RemainStartLevelTime = ChasingPeriodStartCountDown;
        RemainLevelTime = LevelTime;
        IsPreparePeriod = false;
        IsChasingPeriod = false;
        IsMouseCatched = false;
        MouseGetFoodCount = 0;
        // 重置UI
        if (CatWinUI != null)
        {
            CatWinUI.SetActive(false);
        }
        if (MouseWinUI != null)
        {
            MouseWinUI.SetActive(false);
        }
        //关卡内各物品状态重置
        //TODO:清空老鼠洞
        CircleManager.instance.ResetAllHole();
        //TODO:食物状态复位

        //跳介绍界面
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
        // 老鼠洞数据发送
        EventCenter.TriggerEvent(EventKey.HoleCreateDown, CircleManager.instance.GetHoles());
        StartCoroutine(LevelTimeCountDown());
        IEnumerator LevelTimeCountDown()
        {
            while(RemainStartLevelTime > 0.0f)
            {
                RemainStartLevelTime--;
                yield return new WaitForSeconds(1.0f);
            }
            // TODO: 开玩家控制器
            EventCenter.TriggerEvent(EventKey.GameStart);
            while (RemainLevelTime > 0.0f && MouseGetFoodCount < MouseNeedFoodCount && !IsMouseCatched)
            {
                RemainLevelTime--;
                yield return new WaitForSeconds(1.0f);
            }
            IsChasingPeriod = false;
            // TODO: 发事件，关玩家控制器
            EventCenter.TriggerEvent(EventKey.GameEnd);
            // TODO: 跳结算界面
            GameEndAndConclusion();
        }
    }

    void GameEndAndConclusion()
    {
        IsGaming = false;
        if(IsMouseCatched || MouseNeedFoodCount > MouseGetFoodCount )
        {
            // 猫赢
            if(CatWinUI != null)
            {
                CatWinUI.SetActive(true);
            }
        }
        else
        {
            //老鼠赢
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
