using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cheese : MonoBehaviour
{

    [Label("��ťͼ��")]
    public GameObject ButtonHint;

    [Label("�Ƿ���Ա�͵")]
    public bool CanBeStolen;

    [Label("�Ƿ����ڱ�͵")]
    public bool IsStealing;

    [Label("��͵�Ľ���")]
    public float StealProgress = 0;

    [Label("��͵���ٶ�")]
    public float StealSpeed = 10.0f;

    [Label("͵�Ľ�����")]
    public Image ProgressBar;


    private void Start()
    {
        if (ButtonHint != null)
            ButtonHint.SetActive(false);

        CanBeStolen = false;

        //��ʼ͵
        EventCenter.AddSingleEventListener(EventKey.MouseStartSteal, () => 
        { 
            if(CanBeStolen)
            {
                IsStealing = true;
                Debug.Log("Start Steal");
            }
                 
        });

        //����͵
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
            //TODO:�����¼� ������+1
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