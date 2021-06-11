using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class FloatingObject : MonoBehaviour
{
    private bool floatup;
    [Range (0f,.1f)]
    public float speed = .2f;
    [Range (0,5)]
    public int time = 1;

    private void Start()
    {
        floatup = false;
    }

    private void FixedUpdate()
    {
        if (floatup)
           FloatingUp();
        else if (!floatup)
           FloatingDown();
    }

    private void FloatingUp()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        StartCoroutine(Wait(false));
    }

    private void FloatingDown()
    {
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
        StartCoroutine(Wait(true));
    }

    private IEnumerator Wait(bool state)
    {
        yield return new WaitForSecondsRealtime(time);
        floatup = state;
    }
}

