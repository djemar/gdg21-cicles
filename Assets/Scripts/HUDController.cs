using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    public static GameObject TopItem;
    public static GameObject BottomItem;
    public static GameObject LeftItem;
    public static GameObject RightItem;
    public static GameObject Bubble;
    public static GameObject Dance;
    public static GameObject Hammer;
    public static GameObject Bazooka;

    // Start is called before the first frame update
    void Start()
    {

        TopItem = GameObject.Find("Spell");
        BottomItem = GameObject.Find("Estus");
        LeftItem = GameObject.Find("Shield");
        RightItem = GameObject.Find("Weapon");
        Bubble = GameObject.Find("Bubble");
        Dance = GameObject.Find("Dance");
        Hammer = GameObject.Find("Hammer");
        Bazooka = GameObject.Find("Bazooka");

        Bubble.SetActive(false);
        Dance.SetActive(false);
        Hammer.SetActive(false);
        Bazooka.SetActive(false);

    }

    public static void activateTopItem()
    {

        TopItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        Bubble.SetActive(true);

    }
    
    public static void deactivateTopItem()
    {

        TopItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Bubble.SetActive(false); //oppure Bubble.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.3921569f); se vogliamo che la prima volta non ci sia e le altre sia trasparente

    }

    public static void activateBottomItem()
    {

        BottomItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        Dance.SetActive(true);

    }

    public static void deactivateBottomItem()
    {

        BottomItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Dance.SetActive(false);

    }

    public static void activateLeftItem()
    {

        LeftItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        Bazooka.SetActive(true);

    }

    public static void deactivateLeftItem()
    {

        LeftItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Bazooka.SetActive(false);

    }

    public static void activateRightItem()
    {

        RightItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        Hammer.SetActive(true);

    }

    public static void deactivateRightItem()
    {

        RightItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Hammer.SetActive(false);

    }

}
