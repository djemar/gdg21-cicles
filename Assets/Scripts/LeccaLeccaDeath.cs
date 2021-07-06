using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeccaLeccaDeath : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
            coll.gameObject.GetComponent<PlayerCombat>().TakeDamage();
    }
}
