using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPBarController : MonoBehaviour
{
    private GameObject player;
    private StatsController statsController;

    public TMPro.TextMeshProUGUI currentLevel;
    public TMPro.TextMeshProUGUI currentXP;
    public TMPro.TextMeshProUGUI nextLevelXP;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        statsController = player.GetComponent<StatsController>();
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        SetLevel(statsController.statsConfig.currentLevel);
        SetXP(statsController.statsConfig.currentXP);
        SetMinMax(statsController.statsConfig.previousLevelXP, statsController.statsConfig.nextLevelXP);
    }

    private void SetLevel(int level)
    {
        currentLevel.text = level.ToString();
    }

    private void SetXP(int xp)
    {
        slider.value = xp;
        currentXP.text = xp.ToString();
    }

    private void SetMinMax(int previousLevelXP, int xp)
    {
        slider.minValue = previousLevelXP;
        slider.maxValue = xp;
        nextLevelXP.text = xp.ToString();
    }
}
