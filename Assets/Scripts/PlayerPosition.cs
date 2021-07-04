using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerPosition : MonoBehaviour
{
    private GameMaster gm;
    public PlayerCombat playerCombat;
    private CharacterController charController;

    // Start is called before the first frame update

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        charController = GetComponent<CharacterController>();
        charController.transform.position = gm.lastCheckPointPos;
        Debug.Log("Respawn pos at " + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCombat.isDead)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
