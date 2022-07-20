using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MetaUpgrade
{
    public string weaponClassName;
    public string weaponName;
    public List<WeaponMetaUpgradeSO> upgrades;
    public List<Links> connections;
}

[System.Serializable]
public class Links
{
    public int index;
    public int right;
    public int up;
    public int down;
}

public class MetaUpgradeManager : MonoBehaviour
{
    public int gold = 0;
    [SerializeField]
    public List<MetaUpgrade> metaUpgrades;
    public static MetaUpgradeManager instance;

    private void Awake()
    {
        if (instance != null)
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

    public void PurchaseMetaUpgrade(WeaponMetaUpgradeSO upgrade)
    {
        upgrade.purchased = true;
    }
}
