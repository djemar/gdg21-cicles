using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private static RawImage HealthImage;

    private static int HealthValue;
    [SerializeField] private Texture[] Healths;
    private static Texture[] statHealths;

    private static void SetHealth(int value)
    {

        HealthImage.texture = statHealths[value];
        if(value == 5) HealthImage.color = Color.red ;

    }

    public static void IncreaseHealth()
    {

        if (HealthValue < 5)
        {
            HealthValue++;
            SetHealth(HealthValue);
        }

    }

    public static void DecreaseHealth()
    {

        if (HealthValue > 0)
        {
            HealthValue--;
            SetHealth(HealthValue);
        }

    }

    private void Start()
    {
        HealthImage = GetComponent<RawImage>();
        statHealths = Healths;
        HealthValue = 5;
    }

}
