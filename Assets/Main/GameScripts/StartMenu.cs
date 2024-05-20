using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadInstructions()
    {
        SceneManager.LoadScene("Instructions Scene");
    }

    public void LoadOptions()
    {
        SceneManager.LoadScene("Options Scene");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Start Screen");
    }
}
