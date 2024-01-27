using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehavior : MonoBehaviour
{
    public GameObject commonHole;
    public GameObject activeHole;

    public void SetToCommon()
    {
        commonHole.SetActive(true);
        activeHole.SetActive(false);
    }

    public void SetToActive()
    {
        commonHole.SetActive(false);
        commonHole.SetActive(true);
    }
}
