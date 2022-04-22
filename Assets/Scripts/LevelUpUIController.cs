using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIController : MonoBehaviour
{
    public bool visible = false;
    private CanvasGroup group;
    public GameObject choicePrefab;
    private GameObject player;
    private StatsController statsController;
    public GameObject buttonContainer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        statsController = player.GetComponent<StatsController>();
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
        }else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    public void Show()
    {
        if (visible) ClearUpgrades(); //already visible so just reroll upgrades
        visible = true;
        Time.timeScale = 0;
        SelectUpgrades();
    }

    public void Hide()
    {
        visible = false;
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
