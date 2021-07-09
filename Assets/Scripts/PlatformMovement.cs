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
    private bool left;
    private bool right;
    public bool ascending;
    public bool horizontal;
    public float speed;

    void Start()
    {
        if (ascending)
        {
            startPosition = transform.position.y;
            endPosition = startPosition + 20;
        }
        else
        {
            endPosition = transform.position.y;
            startPosition = endPosition - 20;
        }
        if (horizontal)
        {
            startPosition = transform.position.z;
            endPosition = startPosition + 20;
        }
        rising = false;
        descending = false;
        left = false;
        right = false;
    }

    void FixedUpdate()
    {
        if (!horizontal)
        {
            if (!rising && !descending) if (ascending) rising = true; else descending = true;
            if (rising)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, endPosition, transform.position.z), speed * Time.deltaTime);
                if (transform.position.y >= endPosition)
                {
                    rising = false;
                    descending = true;
                }
            }
            if (descending)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, startPosition, transform.position.z), speed * Time.deltaTime);
                if (transform.position.y <= startPosition)
                {
                    rising = true;
                    descending = false;
                }
            }
        }
        else
        {
            if (!left && !right) right = true;
            if(right)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, endPosition), speed * Time.deltaTime);
                if (transform.position.z >= endPosition)
                {
                    right = false;
                    left = true;
                }
            }
            if(left)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, startPosition), speed * Time.deltaTime);
                if (transform.position.z <= startPosition)
                {
                    right = true;
                    left = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.parent = null;
    }

}
