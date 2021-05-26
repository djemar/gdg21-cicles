using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class StaminaUI : MonoBehaviour
{
    public Slider staminaBar;
    public static StaminaUI instance;

    public bool canGlide = true;
    private int maxStamina = 100;
    private int currentStamina;
    private WaitForSeconds regenTick = new WaitForSeconds(0.0001f);

    private Coroutine regen;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        canGlide = true;
    }

    public void UseStamina(int amount)
    {
        if (currentStamina - amount >= 0)
        {
            canGlide = true;
            currentStamina -= amount;
            staminaBar.value = currentStamina;
            if (regen != null)
                StopCoroutine(regen);
            regen = StartCoroutine(RegenStamina());
        }
        else
        {
            Debug.Log("Not enough stamina fat ass");
            canGlide = false;
        }
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(1);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;
            yield return regenTick;
        }
        canGlide = true;
        regen = null;
    }
}