using UnityEngine;
using UnityEngine.UI;

public class StaminaGlidePosition : MonoBehaviour
{
    public Slider gliderStamina;
    Vector3 targetPos;

    void FixedUpdate()
    {

        targetPos = Camera.main.WorldToScreenPoint(this.transform.position);
        gliderStamina.transform.position = targetPos;

    }
}