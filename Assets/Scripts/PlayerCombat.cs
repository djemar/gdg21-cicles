using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject MainMenu;
    public GameObject Count;
    public MaterialManager materialManager;
    public HUDController HUD;
    public bool isAttacking = false;
    public int countMelee = 3;
    public GameObject hammer;
    public float attackRange = 0.5f;
    public float attackRate = 1.75f;
    public bool isDead = false;
    public bool hasWeapon = false;
    public bool hasShield = false;
    float nextAttackTime = 0f;
    public bool isShooting = false;
    public bool endMelee = true;

    public float fireRate = 10f;

    public Transform shootPoint;

    public ParticleSystem shieldEffect;


    [SerializeField]
    private Transform pfBullet;

    private float nextTimetoFire = 0;

    private void Awake()
    {
        MainMenu.GetComponentInChildren<Text>().text = "PAUSE";
        MainMenu.GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttacking)
        {
            if (!isShooting && Time.time >= nextAttackTime)
            {
                StartCoroutine(Attack());
                nextAttackTime = Time.time + 1f / attackRate;
            }
            else if (isShooting && Time.time >= nextTimetoFire)
            {
                nextTimetoFire = Time.time + 1f / fireRate;
                Shoot();

            }
        }
        if (countMelee == 0)
        {
            hasWeapon = false;
            HUD.hasHammer = false;
            Destroy(hammer);
        }
    }

    private IEnumerator Attack()
    {
        //animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
        animator.SetTrigger("Attack");
        isAttacking = false;
        endMelee = false;
        yield return new WaitForSeconds(attackRate);
    }

    private void Shoot()
    {
        // RaycastHit hit;
        // isAttacking = false;

        // if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, shootRange)){
        //     Debug.Log(hit.transform.name);
        // }
        Debug.Log("Trying to shoot");
        isAttacking = false;

        Vector3 shootDir = transform.forward;
        Transform bulletTransform = Instantiate(pfBullet, shootPoint.transform.position, Quaternion.identity);
        bulletTransform.GetComponent<Bullet>().Setup(shootDir);
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started && !isAttacking && (hasWeapon || isShooting))
        {
            isAttacking = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void MeleeAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage();
            countMelee--;
            Count.GetComponent<Text>().text = countMelee.ToString();
        }
    }

    private void EndMeleeAttack()
    {
        endMelee = true;
    }

    public void TakeDamage()
    {
        if (!isDead && !hasShield)
        {
            StartCoroutine(Die());
        }
        else if (hasShield)
        {
            StartCoroutine(Invincibility());
        }
    }

    private IEnumerator Invincibility()
    {
        shieldEffect.Play();
        materialManager.RemoveShield();
        FindObjectOfType<AudioManager>().Play("PowerDown");
        yield return new WaitForSecondsRealtime(1f);
        HUD.hasShield = false;
        hasShield = false;
    }

    private IEnumerator Die()
    {
        animator.SetTrigger("isDead");
        MainMenu.GetComponentInChildren<Text>().text = "YOU DIED";
        MainMenu.GetComponentInChildren<Text>().color = new Color(0.96f, 0.51f, 0.87f);
        MainMenu.SetActive(true);
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSecondsRealtime(3f);
        isDead = true;
    }
}
