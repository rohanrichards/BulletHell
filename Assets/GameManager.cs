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

    public bool testingEnabled = false;
    public BaseItemFixtureSO itemFixture;
    public BaseLevelFixtureSO levelFixture;
    public BaseMetaFixtureSO metaFixture;

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
        List<MetaUpgrade> metaUpgrades = GetComponent<MetaUpgradeManager>().metaUpgrades;
        foreach (MetaUpgrade metaUpgrade in metaUpgrades)
        {
            foreach (WeaponMetaUpgradeSO upgrade in metaUpgrade.upgrades)
            {
                if (upgrade.purchased)
                {
                    WeaponBase weaponScript = (WeaponBase)playerScripts.GetComponent(metaUpgrade.weaponClassName);
                    weaponScript.metaUpgrades.Add(Instantiate(upgrade));
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        if (testingEnabled) {
            InitTestMetaFixtures();
            InitTestLevelFixtures();
            InitTestItemFixtures();
        } else {
            ItemBase startingItem = (ItemBase)playerScripts.GetComponent<Launcher>();
            startingItem.IncreaseLevel();
            startingItem.Unlock();
            //StartCoroutine(SetWinGameTimer());
        }
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

    private void InitTestItemFixtures()
    {
        if(itemFixture == null)
        {
            return;
        }

        for(int w = 0; w < itemFixture.weapons.Length; w++)
        {
            ItemBase item = (ItemBase)playerScripts.GetComponent(itemFixture.weapons[w].label.ToString());
            for(int i = itemFixture.weapons[w].level; i > 0; i--)
            {
                item.IncreaseLevel();
                item.Unlock();
            }
        }

        for(int s = 0; s < itemFixture.stats.Length; s++)
        {
            ItemBase item = (ItemBase)playerScripts.GetComponent(itemFixture.stats[s].label.ToString());
            for(int i = itemFixture.stats[s].level; i > 0; i--)
            {
                item.IncreaseLevel();
                item.Unlock();
            }
        }

    }

    private void InitTestLevelFixtures()
    {
        if(levelFixture == null)
        {
            return;
        }
        DifficultyManager gameplayScripts = GameObject.Find("GameplayScripts").GetComponent<DifficultyManager>();
        gameplayScripts.secondsPlayedOffset = levelFixture.secondsPlayed;
    }

    private void InitTestMetaFixtures()
    {
        if(metaFixture == null)
        {
            return;
        }
    }

}
