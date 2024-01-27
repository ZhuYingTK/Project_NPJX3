using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatUI : MonoBehaviour
{
    public Image CatchCD;
    public Image ShootCD;
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.AddEventListener<float>(EventKey.CatCatchChange,ChangeCatch);
        EventCenter.AddEventListener<float>(EventKey.CatShootChange,ChangeShoot);
    }

    private void ChangeShoot(float obj)
    {
        ShootCD.fillAmount = obj;
    }

    private void ChangeCatch(float obj)
    {
        CatchCD.fillAmount = obj;
    }
    
}
