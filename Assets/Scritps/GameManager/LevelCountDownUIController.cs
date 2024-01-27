using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelCountDownUIController : MonoBehaviour
{
    public static LevelCountDownUIController instance;

    [Label("倒计时UI")]
    public TextMeshProUGUI CountDownUI;

    [Label("开始倒计时UI")]
    public TextMeshProUGUI StartCountDownUI;

    [Label("老鼠需要的食物总数UI")]
    public TextMeshProUGUI TotalMouseFood;

    [Label("老鼠当前的食物数UI")]
    public TextMeshProUGUI CurMouseFood;

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
        
    }

    private void Update()
    {
        UINumUpdate();
    }

    void UINumUpdate()
    {
        if (!GameManager.instance.IsChasingPeriod)
        {
            if(StartCountDownUI != null)
                StartCountDownUI.gameObject.SetActive(false);
            return;
        }

        if(StartCountDownUI != null)
        {
            if(GameManager.instance.RemainStartLevelTime > 0)
            {
                StartCountDownUI.gameObject.SetActive(true);
                StartCountDownUI.text = GameManager.instance.RemainStartLevelTime.ToString();
            }
            else
            {
                StartCountDownUI.gameObject.SetActive(false);
            }
        }
        if(CountDownUI != null)
        {
            var CountDown = GameManager.instance.RemainLevelTime;
            if (CountDown >= 10) { CountDownUI.text = CountDown.ToString(); }
            else { CountDownUI.text = "0" + CountDown.ToString(); }

        }

        if(TotalMouseFood != null)
        {
            TotalMouseFood.text = GameManager.instance.MouseNeedFoodCount.ToString();
        }

        if(CurMouseFood != null)
        {
            CurMouseFood.text = GameManager.instance.MouseGetFoodCount.ToString();
        }
    }


}
