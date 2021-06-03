using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 10f;

    public float range = 10f;
    public float damage = 10f;

    public float blastRadius = 3f;
    public float explosionForce = 100f;

    private Vector3 initialPos;
    private float totalDistance;

    void Start(){
        this.totalDistance = 0;
    }

    private Vector3 shootDir;
    public void Setup(Vector3 shootDir){
        this.shootDir = shootDir;
        this.initialPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position +=  shootDir * moveSpeed * Time.deltaTime;

        totalDistance = (transform.position - initialPos).magnitude;

        if(totalDistance >= range){
            Destroy(gameObject);
        }


    }

    private void OnTriggerEnter(Collider collider){

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        if(colliders.Length == 0)
            Debug.Log("Empty array!");

        foreach(Collider nearbyObject in colliders){
            Target target = nearbyObject.GetComponent<Target>();
            if(target != null){
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if(rb != null){
                    rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
                }
            }
        }

        colliders = Physics.OverlapSphere(transform.position, blastRadius/2);

        foreach(Collider nearbyObject in colliders){
            Target target = nearbyObject.GetComponent<Target>();
            if(target != null){
                target.Hit(damage);
            }
        }
        /*
        Target target = collider.GetComponent<Target>();
        if(target != null){
            target.Hit(damage);
        }
        */
        Destroy(gameObject);
    }
}
