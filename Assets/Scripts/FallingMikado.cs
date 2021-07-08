using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingMikado : MonoBehaviour
{
    public Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false;
            StartCoroutine(DestroyMikado());
        }
    }

    private IEnumerator DestroyMikado()
    {
        yield return new WaitForSecondsRealtime(3f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage();
    }
}
