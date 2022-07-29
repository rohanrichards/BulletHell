using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseGameUIController : ToggleableUIController
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public void Toggle()
    {
        if (!visible)
        {
            Show();
        }else
        {
            Hide();
        }
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
    }


    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Hide();
        World.DisposeAllWorlds();
        DefaultWorldInitialization.Initialize("Default World", false);
        SceneManager.LoadScene("Main Menu");
    }
}
