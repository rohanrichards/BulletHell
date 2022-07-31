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
    private MetaUpgradeManager metaUpgradeManager;
    private PickupGenerator pickupGenerator;
    private OpenChestUIController chestUIController;
    protected BlobAssetStore blobAssetStore;


    void Start()
    {
        stats = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
        metaUpgradeManager = MetaUpgradeManager.instance;
        pickupGenerator = GameObject.Find("GameplayScripts").GetComponent<PickupGenerator>();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        chestUIController = GameObject.FindObjectOfType<OpenChestUIController>();

        entityQuery = manager.CreateEntityQuery(
            ComponentType.ReadWrite<MessageDataComponent>(),
            ComponentType.ReadWrite<EntityDataComponent>()
        );

        blobAssetStore = new BlobAssetStore();

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
                    GameObject explosionGameObject = Instantiate(explosionDeathPrefab, message.position, message.rotation);
                    explosionGameObject.transform.localScale *= type.Size;
                    Entity explosionEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(explosionGameObject, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
                    LightDataComponent light = manager.GetComponentData<LightDataComponent>(explosionEntity);
                    light.radius *= type.Size;
                    light.intensity *= type.Size / 2;
                    manager.SetComponentData<LightDataComponent>(explosionEntity, light);
                    Entity exploder = manager.CreateEntity();
                    manager.AddComponent<ExplodeAndDeleteTag>(exploder);
                    manager.AddComponentData(exploder, type);
                    manager.AddComponentData(exploder, new Translation { Value = message.position });
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

                if (type.Coin != 0)
                {
                    pickupGenerator.CreateCoin(message.position, Mathf.CeilToInt(type.Coin));
                }

                if (type.Chest)
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
                if (type.Coin > 0)
                {
                    metaUpgradeManager.gold += Mathf.CeilToInt(type.Coin);
                }
                if (type.Chest)
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

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}
