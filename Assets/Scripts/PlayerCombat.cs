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
    public MaterialManager materialManager;
    public HUDController HUD;
    public bool isAttacking = false;
    public GameObject hammer;
    public float attackRange = 0.5f;
    public float attackRate = 1.75f;
    public bool isDead = false;
    public bool hasWeapon = false;
    public bool hasShield = false;
    float nextAttackTime = 0f;

    public ParticleSystem shieldEffect;

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
            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(Attack());
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private IEnumerator Attack()
    {
        //animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
        animator.SetTrigger("Attack");
        isAttacking = false;
        yield return new WaitForSeconds(attackRate);
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started && !isAttacking && hasWeapon)
        {
            isAttacking = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Swing()
    {
        FindObjectOfType<AudioManager>().Play("Swing");
    }

    private void MeleeAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage();
        }

        if(hitEnemies.Length != 0) FindObjectOfType<AudioManager>().Play("Hit");
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

    public IEnumerator Die()
    {
        FindObjectOfType<AudioManager>().Play("Die");
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
