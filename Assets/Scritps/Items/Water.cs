using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float slipTime = 3;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cat"))
        {
            CatController controller;
            if (other.TryGetComponent(out controller))
            {
                controller.OnSlip(slipTime);
            }
        }

        if (other.CompareTag("Mouse"))
        {
            MouseController controller;
            if (other.TryGetComponent(out controller))
            {
                //controller.OnSlip(slipTime);
            }
        }
    }
}
