using System.Collections;
using System;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float x;
    public float y;
    public float z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(x,y,z));
    }
}
