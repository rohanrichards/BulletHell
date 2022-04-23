using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.Rendering.Universal;

public class StatsController : MonoBehaviour
{
    public StatsConfigSO statsConfig;
    public GlobalStatsConfigSO globalStatsConfig;

    //availableUpgrades should only ever contain upgrades for weapons we have
    private LevelUpUIController levelUpUIController;
    public List<UpgradeSO> availableUpgrades = new List<UpgradeSO>();
    private ParticleSystem damageNotifier;
    private Animation damageFlashLight;
    DifficultyManager difficultyManager;
    GameManager gameManager;
    Rigidbody2D playerBody;

    public float MoveSpeed
    {
        get { return statsConfig.MoveSpeed; }
    }
    public float RotateSpeed
    {
        get { return statsConfig.RotateSpeed; }
    }
    public float CurrentXP
    {
        get { return statsConfig.currentXP; }
    }

    void Start()
    {
        statsConfig = Instantiate(statsConfig);
        globalStatsConfig = Instantiate(globalStatsConfig);
        levelUpUIController = GameObject.Find("LevelUpPanel").GetComponent<LevelUpUIController>();
        difficultyManager = GameObject.FindObjectOfType<DifficultyManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        damageNotifier = GetComponentInChildren<ParticleSystem>();
        damageFlashLight = GetComponentInChildren<Animation>();
        playerBody = GetComponentInChildren<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LevelUp();
        }
    }

    public void ApplyXP(int XP)
    {
        statsConfig.currentXP += Mathf.CeilToInt(XP + XP * (statsConfig.XPPercentBonus / 100));
        CheckForLevelUp();
    }

    void CheckForLevelUp()
    {
        if(CurrentXP >= statsConfig.nextLevelXP)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        int baseLevelRequirement = 400;
        int newPreviousLevel = statsConfig.nextLevelXP;
        int formulaResult = Mathf.RoundToInt(0.3f * Mathf.Pow(statsConfig.currentLevel, 3));
        Debug.Log("Level increase formula result: " + formulaResult);
        statsConfig.nextLevelXP += baseLevelRequirement + formulaResult;
        statsConfig.previousLevelXP = newPreviousLevel;
        statsConfig.currentLevel++;

        Debug.Log("Leveled up! New Level: " + statsConfig.currentLevel);
        levelUpUIController.Show();
    }
    
    // used to apply damage to the health of the attached game object
    public void ApplyDamage(int damage, EnemyBase source)
    {
        statsConfig.currentHealth -= damage;
        CheckIfDead();
        PlayDamageNotifier(source);
    }

    public void ApplyHealth(int health)
    {
        statsConfig.currentHealth += health;
        if (statsConfig.currentHealth > statsConfig.MaxHealth) statsConfig.currentHealth = statsConfig.MaxHealth;
    }

    public void PlayDamageNotifier(EnemyBase source)
    {
        damageNotifier.transform.LookAt(source.rb.transform, transform.up);
        damageNotifier.transform.Rotate(new Vector3(0, -90, 0));
        damageNotifier.Play();
        damageFlashLight.Play();
    }

    void CheckIfDead()
    {
        if(statsConfig.currentHealth <= 0)
        {
            gameManager.EndGame(false);
        }
    }

    public void SaveStats()
    {
        string stats = JsonUtility.ToJson(statsConfig);
        string statsPath = Application.persistentDataPath + "/stats.json";
        string globalStats = JsonUtility.ToJson(globalStatsConfig);
        string globalStatsPath = Application.persistentDataPath + "/global-stats.json";
        File.WriteAllText(statsPath, stats);
        File.WriteAllText(globalStatsPath, globalStats);
    }

    public void LoadStats()
    {
        string path = Application.persistentDataPath + "/stats.json";
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            StatsConfigSO loadedStats = Instantiate<StatsConfigSO>(statsConfig);
            JsonUtility.FromJsonOverwrite(text, loadedStats);
            statsConfig = loadedStats;
        }
    }

    public List<ItemBase> GenerateUpgradeList()
    {
        System.Random rng = new System.Random();
        // filter out any that don't have levels available
        List<ItemBase> filtered = MakeWeightedList(GetComponents<ItemBase>().ToList()).Where(item => item.levelUpgrades.Length > item.level).ToList();
        // shuffle them
        List<ItemBase> shuffled = filtered.OrderBy(item => rng.Next()).Distinct().ToList();
        return shuffled;
    }

    private List<ItemBase> MakeWeightedList(List<ItemBase> items)
    {
        List<ItemBase> weighted = new List<ItemBase>();
        foreach (ItemBase item in items)
        {
            for (int i = 0; i < item.rarity; i++)
            {
                weighted.Add(item);
                if (item.isUnlocked)
                {
                    //unlocked items are twice as likely to appear
                    weighted.Add(item);
                }
            }
        }
        return weighted;
    }
}
