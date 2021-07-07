using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BubblePlatformPop(int valid)
    {
        Debug.Log("pop");
        Destroy(GameObject.Find("BubblePlatform"));
    }
}
