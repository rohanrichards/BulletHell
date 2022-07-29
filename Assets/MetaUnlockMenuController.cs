using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetaUnlockMenuController : ToggleableUIController
{

    MetaUpgradeManager metaUpgradeManager;
    public GameObject weaponsListContainer;
    public GameObject upgradeContainerPrefab;
    public GameObject upgradeButtonPrefab;
    public override void Start()
    {
        base.Start();
        metaUpgradeManager = MetaUpgradeManager.instance;
        List<MetaUpgrade> weaponMetaUpgrades = metaUpgradeManager.metaUpgrades;

        foreach (Transform child in weaponsListContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (MetaUpgrade metaInfo in weaponMetaUpgrades)
        {
            GameObject prefab = Instantiate(upgradeContainerPrefab, weaponsListContainer.transform);
            GameObject labelGO = prefab.transform.Find("UpgradeLabel").gameObject;
            labelGO.GetComponent<TMPro.TextMeshProUGUI>().text = metaInfo.weaponName;
            GameObject listContainer = prefab.transform.Find("UpgradeList").gameObject;
            foreach (Transform child in listContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            int i = 1;
            foreach (WeaponMetaUpgradeSO metaUpgrade in metaInfo.upgrades)
            {
                GameObject buttonPrefab = Instantiate(upgradeButtonPrefab, listContainer.transform);
                Button button = buttonPrefab.GetComponent<Button>();
                button.onClick.AddListener(delegate { PurchaseUpgrade(metaUpgrade, button); });
                button.transform.Find("UpgradeIndex").GetComponent<TMPro.TextMeshProUGUI>().text = i.ToString();
                i++;
            }
        }

        ResetPurchases(false);
    }

    public void PurchaseUpgrade(WeaponMetaUpgradeSO upgrade, Button button)
    {
        if(upgrade.cost <= metaUpgradeManager.gold)
        {
            metaUpgradeManager.gold -= upgrade.cost;
            upgrade.purchased = true;
        }
    }

    public void ResetPurchases(bool refund = true)
    {
        List<MetaUpgrade> weaponMetaUpgrades = metaUpgradeManager.metaUpgrades;

        foreach (MetaUpgrade metaInfo in weaponMetaUpgrades)
        {
            foreach (WeaponMetaUpgradeSO metaUpgrade in metaInfo.upgrades)
            {
                metaUpgrade.purchased = false;
                if (refund)
                {
                    metaUpgradeManager.gold += metaUpgrade.cost;
                }
            }
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateButtonState();
        transform.Find("GoldLabel").GetComponent<TMPro.TextMeshProUGUI>().text = "Gold: " + metaUpgradeManager.gold;
    }

    public void UpdateButtonState()
    {
        List<MetaUpgrade> weaponMetaUpgrades = metaUpgradeManager.metaUpgrades;
        int metaIndex = 0;

        foreach (MetaUpgrade metaInfo in weaponMetaUpgrades)
        {
            GameObject prefab = weaponsListContainer.transform.GetChild(metaIndex).gameObject;
            GameObject listContainer = prefab.transform.Find("UpgradeList").gameObject;
            int upgradeIndex = 0;
            int highestPurchasedIndex = -1;

            foreach (WeaponMetaUpgradeSO metaUpgrade in metaInfo.upgrades)
            {
                GameObject buttonPrefab = listContainer.transform.GetChild(upgradeIndex).gameObject;
                Button button = buttonPrefab.GetComponent<Button>();
                if (metaUpgrade.purchased)
                {
                    button.interactable = false;
                    button.image.color = new Color { r = 0.5f, g = 1f, b = 0.5f, a = 1f };
                    highestPurchasedIndex = upgradeIndex;
                }
                else
                {
                    if (upgradeIndex - 1 > highestPurchasedIndex)
                    {
                        button.interactable = false;
                    }
                    else
                    {
                        button.interactable = true;
                    }
                }
                upgradeIndex++;
            }
            metaIndex++;
        }
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
