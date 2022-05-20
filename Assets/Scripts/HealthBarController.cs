using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private StatsController statsController;
    private Slider slider;
    void Start()
    {
        statsController = GameObject.FindObjectOfType<StatsController>();
        slider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float maxHealth = ECSPlayerController.getPlayerHealth().MaxHealth;
        float currentHealth = ECSPlayerController.getPlayerHealth().CurrentHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }
}
