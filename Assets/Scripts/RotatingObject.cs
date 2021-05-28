using System.Collections;
using System;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f,1f,0f));
    }
}
