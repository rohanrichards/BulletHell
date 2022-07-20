using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIController : ToggleableUIController
{
    public GameObject choicePrefab;
    private StatsController statsController;
    public GameObject buttonContainer;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Show()
    {
        base.Show();
        if (visible) ClearUpgrades(); //already visible so just reroll upgrades
        Time.timeScale = 0;
        SelectUpgrades();
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1;
    }

    private void SelectUpgrades()
    {
        List<ItemBase> shuffled = statsController.GenerateUpgradeList();

        if (shuffled.Count == 0)
        {
            // no upgrades left
            Hide();
            return;
        }
        
        // now create the buttons we need for the UI
        for(int i = 0; i < shuffled.Count && i < 3; i++)
        {
            ItemBase item = shuffled[i];
            GameObject choiceButton = Instantiate<GameObject>(choicePrefab, buttonContainer.transform);
            UpgradeSO nextLevel = item.levelUpgrades[item.level];
            GameObject nameText = choiceButton.transform.Find("Text/UpgradeName").gameObject;
            GameObject descriptionText = choiceButton.transform.Find("Text/UpgradeDescription").gameObject;
            nameText.GetComponent<TMPro.TextMeshProUGUI>().text = nextLevel.upgradeName;
            descriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = nextLevel.upgradeDescription;
            choiceButton.GetComponent<Button>().onClick.AddListener(delegate { ApplyUpgrade(item); });
        }
    }

    public void ClearUpgrades()
    {
        foreach (Transform child in buttonContainer.transform) Destroy(child.gameObject);
    }

    void ApplyUpgrade(ItemBase selectedItem)
    {
        if(selectedItem.level == 0)
        {
            selectedItem.Unlock();
        }
        selectedItem.IncreaseLevel();

        ClearUpgrades();
        Hide();
    }
}
