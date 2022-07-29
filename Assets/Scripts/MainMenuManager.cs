using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : ToggleableUIController
{
    public GameObject mechSelectorPrefab;
    public GameObject mechSelectorContainer;
    public int defaultSelectedMechIndex = 1;
    private GameManager gameManager;

    public override void Start()
    {
        base.Start();
        visible = true;
        gameManager = GameManager.instance;
        List<MechConfig> configs = gameManager.mechConfigs;

        // make sure the mech button container is empty
        foreach (Transform child in mechSelectorContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //create a mech selection button for each mech we have available
        for (int i = 0; i < configs.Count; i++)
        {
            MechConfig config = configs[i];
            GameObject button = Instantiate(mechSelectorPrefab, mechSelectorContainer.transform);
            GameObject textGO = button.transform.Find("MechName").gameObject;
            textGO.GetComponent<TMPro.TextMeshProUGUI>().text = config.title;
            button.GetComponent<Button>().onClick.AddListener(delegate { SelectMech(config, button); });

            if(i == defaultSelectedMechIndex)
            {
                button.GetComponent<Button>().Select();
                SelectMech(config, button);
            }
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public void SelectMech(MechConfig config, GameObject button)
    {
        gameManager.startingPlayerStatsConfig = config.statsConfig;
        gameManager.startingPlayerGlobalStatsConfig = config.globalStatsConfig;
        Debug.Log("set selected mech to: " + config.title);
    }
    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void NewGame()
    {
        GameManager.instance.NewGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
