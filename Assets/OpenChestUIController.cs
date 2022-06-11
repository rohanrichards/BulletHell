using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChestUIController : MonoBehaviour
{
    public bool visible = false;
    private CanvasGroup group;
    private StatsController statsController;
    public GameObject itemDisplay;

    // Start is called before the first frame update
    void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>();
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (visible)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
    public void Show()
    {
        visible = true;
        Time.timeScale = 0;
        SelectRandomUpgrade();
    }

    public void Hide()
    {
        visible = false;
        Time.timeScale = 1;
    }

    private void SelectRandomUpgrade() {
        List<ItemBase> shuffled = statsController.GenerateUpgradeList();
        ItemBase item = shuffled[0];

        GameObject nameText = itemDisplay.transform.Find("UpgradeName").gameObject;
        UpgradeSO nextLevel = item.levelUpgrades[item.level];
        nameText.GetComponent<TMPro.TextMeshProUGUI>().text = nextLevel.upgradeName;

        ApplyUpgrade(item);
    }

    void ApplyUpgrade(ItemBase selectedItem)
    {
        if (selectedItem.level == 0)
        {
            selectedItem.Unlock();
        }
        selectedItem.IncreaseLevel();
    }
}
