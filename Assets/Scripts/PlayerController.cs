using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerController : MonoBehaviour, IConvertGameObjectToEntity
{
    private StatsController statsController;

    // Start is called before the first frame update
    void Awake()
    {
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
    }

    void FixedUpdate()
    {

    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.FindObjectOfType<PauseGameUIController>().Toggle();
        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            statsController.SaveStats();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            statsController.LoadStats();
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();

        // set up player components
        dstManager.AddComponent(entity, typeof(PlayerTag));

        dstManager.AddComponentData(entity, new EntityMovementSettings { moveSpeed = statsController.MoveSpeed });
        dstManager.AddComponentData(entity, new EntityXPComponent { CurrentXP = statsController.CurrentXP });
        dstManager.AddComponentData(entity, new EntityDataComponent { Type = EntityDeathTypes.EndsGameOnDeath });
        dstManager.AddComponentData(entity, new EntityHealthComponent { CurrentHealth = statsController.statsConfig.MaxHealth, MaxHealth = statsController.statsConfig.MaxHealth });
    }
}
