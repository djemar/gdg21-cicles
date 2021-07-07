using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUDController : MonoBehaviour
{
    public static GameObject TopItem;
    public static GameObject BottomItem;
    public static GameObject LeftItem;
    public static GameObject RightItem;
    public GameObject Bubble;
    public GameObject Dance;
    public GameObject Residual;
    public GameObject Hammer;
    public GameObject UnactiveBubble;
    public GameObject UnactiveDance;
    public GameObject UnactiveResidual;
    public GameObject UnactiveHammer;
    private Vector2 inputVector;
    private Vector2 Up = new Vector2(0, 1);
    private Vector2 Down = new Vector2(0, -1);
    private Vector2 Left = new Vector2(-1, 0);
    private Vector2 Right = new Vector2(1, 0);
    public PlayerCombat playerCombat;
    public MaterialManager materialManager;
    private GameObject MeleeWeapon;
    private GameObject Shield;
    private GameObject Trampoline;
    private GameObject ResidualImage;

    public Vector3 PickPosition;    // x = 0.003419     y = 0.000451    z = 0.000457
    public Vector3 PickRotation;    // x = 82.979       y = -8.463      z = 86.094

    public Transform Hand;

    public Transform WeaponPosition;
    public bool hasHammer = false;
    public bool hasShield = false;
    public bool hasResidual = false;
    public bool hasBubble = false;
    public GameObject bubblePlatform;
    public GameObject resImage;


    public ParticleSystem shieldEffect;
    private GameMaster gm;

    private Animator bubbleAnim;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();

        TopItem = GameObject.Find("TopItem");
        BottomItem = GameObject.Find("BottomItem");
        LeftItem = GameObject.Find("LeftItem");
        RightItem = GameObject.Find("RightItem");

        Bubble.SetActive(false);
        Dance.SetActive(false);
        Hammer.SetActive(false);
        Residual.SetActive(false);
        UnactiveBubble.SetActive(false);
        UnactiveDance.SetActive(false);
        UnactiveHammer.SetActive(false);
        UnactiveResidual.SetActive(false);

        bubbleAnim = bubblePlatform.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {

    }

    public void OnInventory(InputAction.CallbackContext value)
    {
        inputVector = value.ReadValue<Vector2>();
        if (inputVector.Equals(Up) && hasShield && value.started) //1
        {
            if (Dance.activeSelf)
            {
                FindObjectOfType<AudioManager>().Play("ApplyShield");
                shieldEffect.Play();
                deactivateTopItem();
                playerCombat.hasShield = true;
                materialManager.ActivateShield();
            }
        }
        else if (inputVector.Equals(Down) && hasBubble && value.started) //2
        {
            if (Bubble.activeSelf)
            {
                deactivateBottomItem();
                var position = transform.position;
                var direction = transform.forward;
                var rotation = transform.rotation;
                float distance = 1.5f;
                Vector3 up = new Vector3(0, 1, 0);
                Vector3 spawnPos = position + direction * distance + up;
                var obj = Instantiate(bubblePlatform, spawnPos, rotation);
                obj.name = "BubblePlatform";
                //bubbleAnim.Play("Inflate");
            }
        }
        else if (inputVector.Equals(Left) && value.started && hasResidual) //3
        {
            if (Residual.activeSelf)
            {
                FindObjectOfType<AudioManager>().Play("Checkpoint");
                var obj = GameObject.FindGameObjectWithTag("Checkpoint");
                if (obj != null)
                {
                    Destroy(obj);
                }
                deactivateLeftItem();
                var position = transform.position;
                var rotation = transform.rotation;
                var image = Instantiate(resImage, position, rotation);
                image.tag = "Checkpoint";
                gm.lastCheckPointPos = position;
                DontDestroyOnLoad(image);
                hasResidual = false;
            }

        }
        else if (inputVector.Equals(Right) && hasHammer && value.started && !playerCombat.isAttacking) //4
        {
            if (Hammer.activeSelf)
            {
                deactivateRightItem();
                playerCombat.hasWeapon = true;
                MeleeWeapon.SetActive(true);
                Debug.Log("Activating hammer: " + MeleeWeapon.activeSelf);
            }
            else if (!Hammer.activeSelf)
            {
                activateRightItem();
                playerCombat.hasWeapon = false;
                MeleeWeapon.SetActive(false);
                Debug.Log("Deactivating hammer: " + MeleeWeapon.activeSelf);
            }
        }
    }

    public void pickUpMeleeWeapon(Collider collision)
    {
        hasHammer = true;
        MeleeWeapon = collision.gameObject;
        MeleeWeapon.tag = "Untagged";
        MeleeWeapon.transform.parent = Hand.transform;
        MeleeWeapon.transform.localPosition = PickPosition;
        MeleeWeapon.transform.localEulerAngles = PickRotation;
        MeleeWeapon.SetActive(false);
        Debug.Log("Picking object: " + MeleeWeapon.activeSelf);
        activateRightItem();
    }

    public void pickUpShield(Collider collision)
    {
        hasShield = true;
        Shield = collision.gameObject;
        Shield.tag = "Untagged";
        Shield.SetActive(false);
        activateTopItem();
    }

    public void pickUpBubble(Collider collision)
    {
        hasBubble = true;
        Trampoline = collision.gameObject;
        Trampoline.tag = "Untagged";
        Trampoline.SetActive(false);
        activateBottomItem();
    }

    public void pickUpResidual(Collider collision)
    {
        hasResidual = true;
        ResidualImage = collision.gameObject;
        ResidualImage.tag = "Untagged";
        ResidualImage.SetActive(false);
        activateLeftItem();
    }

    public void activateBottomItem()
    {

        //TopItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        UnactiveBubble.SetActive(false);
        Bubble.SetActive(true);

    }

    public void deactivateBottomItem()
    {

        //TopItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Bubble.SetActive(false);
        UnactiveBubble.SetActive(true);

    }

    public void activateTopItem()
    {

        //BottomItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        UnactiveDance.SetActive(false);
        Dance.SetActive(true);

    }

    public void deactivateTopItem()
    {

        //BottomItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Dance.SetActive(false);
        UnactiveDance.SetActive(true);

    }

    public void activateLeftItem()
    {

        //LeftItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        UnactiveResidual.SetActive(false);
        Residual.SetActive(true);

    }

    public void deactivateLeftItem()
    {

        //LeftItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Residual.SetActive(false);
        UnactiveResidual.SetActive(true);

    }

    public void activateRightItem()
    {

        //RightItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 1f);
        UnactiveHammer.SetActive(false);
        Hammer.SetActive(true);

    }

    public void deactivateRightItem()
    {

        //RightItem.GetComponent<Image>().color = new Color(1f, 0.5607843f, 0.8747101f, 0.3921569f);
        Hammer.SetActive(false);
        UnactiveHammer.SetActive(true);

    }

}
