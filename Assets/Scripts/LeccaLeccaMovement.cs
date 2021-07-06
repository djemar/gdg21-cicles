using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeccaLeccaMovement : MonoBehaviour
{

    public GameObject Pivot;
    public float speed;
    public bool forward;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(forward) transform.RotateAround(Pivot.transform.position, Vector3.forward, speed * Time.deltaTime);
        else transform.RotateAround(Pivot.transform.position, Vector3.back, speed * Time.deltaTime);
    }
}
