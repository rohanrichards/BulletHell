using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.instance.NewGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
