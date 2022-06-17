using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.Rendering.Universal;
using Unity.Mathematics;
using Unity.Transforms;

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

    public void ApplyHealth(int health)
    {
        float maxHealth = ECSPlayerController.getPlayerHealth().MaxHealth;
        float currentHealth = ECSPlayerController.getPlayerHealth().CurrentHealth;
        currentHealth += health;
        if(currentHealth > maxHealth) { currentHealth = maxHealth; }
        ECSPlayerController.setPlayerHealth(Mathf.CeilToInt(currentHealth));
    }

    public void PlayDamageNotifier(float3 source)
    {
        LocalToWorld playerLocal = ECSPlayerController.getPlayerLocation();
        damageNotifier.transform.LookAt(source, playerLocal.Up);
        damageNotifier.transform.Rotate(new Vector3(0, -90, 0));
        damageNotifier.Play();
        damageFlashLight.Play();
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
