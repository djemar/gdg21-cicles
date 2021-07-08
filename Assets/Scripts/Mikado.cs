using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mikado : MonoBehaviour
{ 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("DEAD ON MIKADO");
            collision.gameObject.GetComponent<PlayerCombat>().isDead = true;
        }
    }
}
