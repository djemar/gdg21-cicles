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
    private bool isAttacking = false;
    public float attackRange = 0.5f;
    public float attackRate = 0.5f;
    private bool isDead = false;
    public bool hasWeapon = false;
    float nextAttackTime = 0f;

    // Update is called once per frame

    void FixedUpdate()
    {
        if (isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        //animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
        animator.SetTrigger("Attack");
        isAttacking = false;
        nextAttackTime = Time.time + 1f / attackRate;
        yield return new WaitForSeconds(0.5f);

        //animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 0);
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started && Time.time >= nextAttackTime && hasWeapon)
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

    public void TakeDamage()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log("Player is dead!");
            animator.SetTrigger("isDead");
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            MainMenu.SetActive(true);
            //Destroy(gameObject, 3f);
        }
    }
}
