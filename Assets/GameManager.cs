using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MechConfig
{
    public string title;
    public StatsConfigSO statsConfig;
    public GlobalStatsConfigSO globalStatsConfig;
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private GameObject playerScripts;
    public int GameLengthInSeconds;
    public StatsConfigSO startingPlayerStatsConfig;
    public GlobalStatsConfigSO startingPlayerGlobalStatsConfig;
    public List<MechConfig> mechConfigs;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
    } 

    void Update()
    {
        
    }

    public void NewGame()
    {
        StartCoroutine(LoadScene());
    }

    public IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level 1");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        StartCoroutine(GameManager.instance.StartNewGame());
    }

    public IEnumerator StartNewGame()
    {
        //set up the player
        playerScripts = GameObject.Find("PlayerScripts");
        playerScripts.GetComponent<ECSPlayerController>().CreatePlayer(startingPlayerStatsConfig, startingPlayerGlobalStatsConfig);
        
        yield return new WaitForSeconds(0.5f);
        ItemBase startingItem = (ItemBase)playerScripts.GetComponent<Laser>();
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
