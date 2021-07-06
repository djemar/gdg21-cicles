using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    private float startPosition;
    private float endPosition;
    private bool rising;
    private bool descending;

    void Start()
    {
        startPosition = transform.position.y;
        endPosition = startPosition + 20;
        rising = false;
        descending = false;
    }

    void FixedUpdate()
    {
        if (!rising && !descending) rising = true;
        if (rising)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, endPosition, transform.position.z), 3 * Time.deltaTime);
            if(transform.position.y >= endPosition) { 
                rising = false;
                descending = true;
            }
        }
        if(descending)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, startPosition, transform.position.z), 3 * Time.deltaTime);
            if (transform.position.y <= startPosition)
            {
                rising = true;
                descending = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.parent = null;
    }

}
