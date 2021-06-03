using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    private bool isAttacking = false;
    
    public bool isShooting = true;

    public float fireRate = 10f;

    public Transform attackPoint;

    public Transform shootPoint;
    public float attackRange = 0.5f;

    public LayerMask enemyLayers;


    [SerializeField]
    private Transform pfBullet;

    private float nextTimetoFire = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttacking)
        {
            if(!isShooting){
            StartCoroutine(Attack());
            }
            else if(Time.time >= nextTimetoFire){
                nextTimetoFire = Time.time + 1f/fireRate;
                Shoot();

            }
        }
    }

    private IEnumerator Attack()
    {
        //animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
        animator.SetTrigger("Attack");
        isAttacking = false;

        yield return new WaitForSeconds(0.5f);
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Destroy(enemy.gameObject);
        }
        //animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 0);
    }

    private void Shoot(){
        // RaycastHit hit;
        // isAttacking = false;

        // if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, shootRange)){
        //     Debug.Log(hit.transform.name);
        // }
        isAttacking = false;

        Vector3 shootDir = transform.forward;
        Transform bulletTransform = Instantiate(pfBullet, shootPoint.transform.position, Quaternion.identity);
        bulletTransform.GetComponent<Bullet>().Setup(shootDir);
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started && !isAttacking)
        {
            isAttacking = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
