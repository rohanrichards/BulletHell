using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIController : MonoBehaviour
{
    public bool visible = false;
    private CanvasGroup group;
    public GameObject choicePrefab;
    private GameObject player;
    public GameObject buttonContainer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        Debug.Log("toggling UI panel");
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
        System.Random rng = new System.Random();
        // filter out any that don't have levels available
        List<ItemBase> filtered = MakeWeightedList(player.GetComponents<ItemBase>().ToList()).Where(item => item.levelUpgrades.Length > item.level).ToList();
        // shuffle them
        List<ItemBase> shuffled = filtered.OrderBy(item => rng.Next()).Distinct().ToList();


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
            GameObject nameText = choiceButton.transform.Find("UpgradeName").gameObject;
            nameText.GetComponent<TMPro.TextMeshProUGUI>().text = nextLevel.upgradeName;
            choiceButton.GetComponent<Button>().onClick.AddListener(delegate { ApplyUpgrade(item); });
        }
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
