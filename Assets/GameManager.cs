using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    public int GameLengthInSeconds;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(StartNewGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartNewGame()
    {
        yield return new WaitForSeconds(1);
        //set up the player
        ItemBase startingItem = (ItemBase)player.GetComponent<Shotgun>();
        startingItem.Unlock();
        startingItem.IncreaseLevel();
        //StartCoroutine(SetWinGameTimer());
    }

    IEnumerator SetWinGameTimer()
    {
        yield return new WaitForSeconds(GameLengthInSeconds);

        //EndGame(true);
    }

    public void EndGame(bool win)
    {
        EndGameUIController ui = GameObject.FindObjectOfType<EndGameUIController>();
        if (win)
        {
            ui.Win();
        }else
        {
            ui.Lose();
        }
    }
}
