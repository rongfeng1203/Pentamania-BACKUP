using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenus : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
        Debug.Log("I clicked you!");   
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("I clicked you!");
    }
}
