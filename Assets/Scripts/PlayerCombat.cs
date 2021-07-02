using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject MainMenu;
    public MaterialManager materialManager;
    public HUDController HUD;
    public bool isAttacking = false;
    public float attackRange = 0.5f;
    public float attackRate = 1.75f;
    private bool isDead = false;
    public bool hasWeapon = false;
    public bool hasShield = false;
    float nextAttackTime = 0f;
    public bool isShooting = false;
    public bool endMelee = false;

    public float fireRate = 10f;

    public Transform shootPoint;


    [SerializeField]
    private Transform pfBullet;

    private float nextTimetoFire = 0;

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
        }
    }

    private void EndMeleeAttack()
    {
        endMelee = true;
        Debug.Log(endMelee);
    }

    public void TakeDamage()
    {
        if (!isDead && !hasShield)
        {
            isDead = true;
            Debug.Log("Player is dead!");
            animator.SetTrigger("isDead");
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            MainMenu.SetActive(true);
            //Destroy(gameObject, 3f);
        }
        else if (hasShield)
        {
            materialManager.RemoveShield();
            FindObjectOfType<AudioManager>().Play("PowerDown");
            hasShield = false;
            HUD.hasShield = false;
        }
    }
}
