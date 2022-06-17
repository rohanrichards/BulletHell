using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

public class EntityMessagingController : MonoBehaviour
{
    public GameObject explosionDeathPrefab;
    public GameObject splatterDeathPrefab;
    public GameObject EnemyAttackPrefab;
    EntityManager manager;
    private EntityQuery entityQuery;
    private StatsController stats;
    private PickupGenerator pickupGenerator;
    private OpenChestUIController chestUIController;

    void Start()
    {
        stats = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
        pickupGenerator = GameObject.Find("GameplayScripts").GetComponent<PickupGenerator>();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        chestUIController = GameObject.FindObjectOfType<OpenChestUIController>();

        entityQuery = manager.CreateEntityQuery(
            ComponentType.ReadWrite<MessageDataComponent>(),
            ComponentType.ReadWrite<EntityDataComponent>()
        );
    }

    void Update()
    {
        NativeArray<MessageDataComponent> entityData = entityQuery.ToComponentDataArray<MessageDataComponent>(Allocator.Temp);
        NativeArray<EntityDataComponent> typeData = entityQuery.ToComponentDataArray<EntityDataComponent>(Allocator.Temp);
        for (int i = 0; i < entityData.Length; i++)
        {
            MessageDataComponent message = entityData[i];
            EntityDataComponent type = typeData[i];

            if (message.type == MessageTypes.Death)
            {
                if (type.Type == EntityDeathTypes.ExplodesOnDeath)
                {
                    Instantiate(explosionDeathPrefab, message.position, message.rotation);
                    Entity exploder = manager.CreateEntity();
                    manager.AddComponent<ExplodeAndDeleteTag>(exploder);

                    manager.AddComponent<EntityDataComponent>(exploder);
                    manager.SetComponentData(exploder, type);

                    manager.AddComponent<Translation>(exploder);
                    manager.SetComponentData(exploder, new Translation { Value = message.position });
                }
                if (type.Type == EntityDeathTypes.SplattersOnDeath)
                {
                    Instantiate(splatterDeathPrefab, message.position, message.rotation);
                }

                if (type.XP != 0)
                {
                    pickupGenerator.CreateXPOrb(message.position, Mathf.CeilToInt(type.XP));
                }

                if(type.Health != 0)
                {
                    pickupGenerator.CreateRepairItem(message.position, Mathf.CeilToInt(type.Health));
                }

                if(type.Chest)
                {
                    pickupGenerator.CreateChest(message.position);
                }
            }
            if(message.type == MessageTypes.Pickup)
            {
                if(type.XP > 0)
                {
                    stats.ApplyXP(Mathf.CeilToInt(type.XP));
                }
                if(type.Health > 0)
                {
                    stats.ApplyHealth(Mathf.CeilToInt(type.Health));
                }
                if(type.Chest)
                {
                    chestUIController.Show();
                }
            }

            if(message.type == MessageTypes.Attack)
            {
                float3 euler = message.rotation.eulerAngles;
                GameObject particleInstance = Instantiate(EnemyAttackPrefab, message.position, message.rotation);
                ParticleSystem.MainModule particles = particleInstance.GetComponent<ParticleSystem>().main;
                particles.startRotation = (euler.z - 90) * Mathf.Deg2Rad;
            }
        }
        manager.DestroyEntity(entityQuery);
    }
}
