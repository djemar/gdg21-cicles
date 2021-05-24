using System.Collections;
using System;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    private bool floatup;

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
        transform.position += new Vector3(0, .2f * Time.deltaTime, 0);
        StartCoroutine(Wait(false));
    }

    private void FloatingDown()
    {
        transform.position -= new Vector3(0, .2f * Time.deltaTime, 0);
        StartCoroutine(Wait(true));
    }

    private IEnumerator Wait(bool state)
    {
        Debug.Log("WaitingBefore");
        yield return new WaitForSecondsRealtime(1);
        floatup = state;
        Debug.Log("WaitingAfter");
    }
}

