using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public Material gum;
    public Material body;
    public GameObject amy;

    public void ActivateShield()
    {
        amy.GetComponent<Renderer>().material = gum;
    }

    public void RemoveShield()
    {
        amy.GetComponent<Renderer>().material = body;
    }

}
