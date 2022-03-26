using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public StatsConfigSO statsConfig;
    public GlobalStatsConfigSO globalStatsConfig;

    //availableUpgrades should only ever contain upgrades for weapons we have
    private LevelUpUIController levelUpUIController;
    public List<UpgradeSO> availableUpgrades = new List<UpgradeSO>();
    DifficultyManager difficultyManager;
    GameManager gameManager;

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
        Debug.Log("Current XP: " + CurrentXP + " Next Level: " + statsConfig.nextLevelXP);
        if(CurrentXP >= statsConfig.nextLevelXP)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        int baseLevelRequirement = 150;
        int newPreviousLevel = statsConfig.nextLevelXP;
        int formulaResult = Mathf.RoundToInt(1.2f * Mathf.Pow(statsConfig.currentLevel, 3));
        Debug.Log("Level increase formula result: " + formulaResult);
        statsConfig.nextLevelXP += baseLevelRequirement + formulaResult;
        statsConfig.previousLevelXP = newPreviousLevel;
        statsConfig.currentLevel++;

        Debug.Log("Leveled up! New Level: " + statsConfig.currentLevel);
        levelUpUIController.Show();
    }
    
    // used to apply damage to the health of the attached game object
    public void ApplyDamage(int damage)
    {
        statsConfig.currentHealth -= damage;
        Debug.Log("Player hit: " + statsConfig.currentHealth);
        CheckIfDead();
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
            Debug.Log(loadedStats);
            statsConfig = loadedStats;
        }
    }
}
