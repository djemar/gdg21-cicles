using UnityEngine;
using System;

public class PlayerAudio : MonoBehaviour
{
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void StepL()
    {
        //audioManager.Play("PlayerStepL");
    }

    private void StepR()
    {
        //audioManager.Play("PlayerStepR");
    }

    private void JumpAudio()
    {
        System.Random rd = new System.Random();
        int flag = rd.Next(1, 5);
        if (flag == 1)
        {
            audioManager.Play("PlayerLaugh01");
        }
        else if (flag == 2)
        {
            audioManager.Play("PlayerLaugh02");
        }
    }

    private void IdleAudio()
    {
        // audioManager.Play("PlayerIdle");
    }

    private void InflateGumAudio()
    {
        // audioManager.Play("InflateGum");
    }
}

