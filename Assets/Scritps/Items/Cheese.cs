using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cheese : MonoBehaviour
{

    [Label("按钮图标")]
    public GameObject ButtonHint;

    [Label("是否可以被偷")]
    public bool CanBeStolen;

    [Label("是否正在被偷")]
    public bool IsStealing;

    [Label("被偷的进度")]
    public float StealProgress = 0;

    [Label("被偷的速度")]
    public float StealSpeed = 10.0f;

    [Label("偷的进度条")]
    public Image ProgressBar;


    private void Start()
    {
        if (ButtonHint != null)
            ButtonHint.SetActive(false);

        CanBeStolen = false;

        //开始偷
        EventCenter.AddSingleEventListener(EventKey.MouseStartSteal, () => 
        { 
            if(CanBeStolen)
            {
                IsStealing = true;
                Debug.Log("Start Steal");
            }
                 
        });

        //结束偷
        EventCenter.AddSingleEventListener(EventKey.MouseEndSteal, () => 
        { 
            if(IsStealing)
            {
                IsStealing = false;
                Debug.Log("End Steal");
            }
            
        });
    }

    private void Update()
    {
        if(IsStealing)
        {
            StealProgress += StealSpeed * Time.deltaTime;
        }

        if(ProgressBar != null)
        {
            ProgressBar.fillAmount = StealProgress / 100f;
        }

        if(StealProgress >= 100f)
        {
            //TODO:发送事件 奶酪数+1
            EventCenter.TriggerEvent(EventKey.MouseGetFood);
            Destroy(this.gameObject);
        }

    }



    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Mouse")
        {
            if (ButtonHint != null)
                ButtonHint.SetActive(true);
            CanBeStolen = true;
        }
    }


    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Mouse")
        {
            if (ButtonHint != null)
                ButtonHint.SetActive(false);
            CanBeStolen = false;
        }
    }

}
