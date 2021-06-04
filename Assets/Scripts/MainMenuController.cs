using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    private int selected; //0 = play, 1 = quit
    private GameObject PlayButton;
    private GameObject QuitButton;

    void Start()
    {

        selected = 0;
        
        PlayButton = GameObject.Find("Play");
        QuitButton = GameObject.Find("Quit");

        PlayButton.GetComponent<Image>().color = new Color(0.96f, 0.96f, 0.96f, 1f);

    }

    public void Play()
    {
        //UnityEngine.Debug.Log("Play");
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlaygroundScene");
    }

    public void Quit()
    {
        //UnityEngine.Debug.Log("Quit");
        Application.Quit();
    }

    public void OnUp()
    {
        //UnityEngine.Debug.Log("Up pressed");
        selected = 0;
        PlayButton.GetComponent<Image>().color = new Color(0.96f, 0.96f, 0.96f, 1f);
        QuitButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnDown()
    {
        //UnityEngine.Debug.Log("Down pressed");
        selected = 1;
        QuitButton.GetComponent<Image>().color = new Color(0.96f, 0.96f, 0.96f, 1f);
        PlayButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnSelect()
    {
        //UnityEngine.Debug.Log("Select pressed");
        switch (selected)
        {
            case 0:
                Play();
                break;
            case 1:
                Quit();
                break;
        }
    }

}
