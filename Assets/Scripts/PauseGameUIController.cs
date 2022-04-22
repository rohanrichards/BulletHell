using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseGameUIController : MonoBehaviour
{
    public bool visible = false;
    private CanvasGroup group;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        group = gameObject.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (visible)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
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

    public void Show()
    {
        visible = true;
        Time.timeScale = 0;
    }


    public void Hide()
    {
        visible = false;
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Hide();
        SceneManager.LoadScene("Main Menu");
    }
}
