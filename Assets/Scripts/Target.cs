using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    private float health = 10;
    
    public void Hit(float damage){
        this.health -= damage;

        if(health <= 0){
            Destroy(gameObject);
        }

    }

}
