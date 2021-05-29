using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void TakeDamage()
    {
        Die();
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        GetComponent<CharacterController>().enabled = false;
        this.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
