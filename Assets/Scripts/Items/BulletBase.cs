using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour, IConvertGameObjectToEntity
{
    public BulletSO config;
    public GameObject deathPrefab;
    [HideInInspector]
    public WeaponBase parentWeapon;
    protected StatsController statsController;
    protected Vector3 originalOffset;
    static protected EntityManager entityManager;

    public float Damage
    {
        get
        {
            return config.Damage + config.Damage * (statsController.globalStatsConfig.damagePercentBonus / 100);
        }
    }

    public float AOE
    {
        get
        {
            return config.AOE + config.AOE * (statsController.globalStatsConfig.areaPercentBonus / 100);
        }
    }

    public static GameObject Create(GameObject prefab, Transform origin, Vector3 offset, Quaternion rotation, Vector3 rotationOffset, BulletSO config, WeaponBase weapon)
    {
        // create our bullet instance
        /*        GameObject bulletInstance = Instantiate<GameObject>(prefab, origin.position + offset, rotation);
                BulletBase controller = bulletInstance.GetComponent<BulletBase>();
                controller.originalOffset = rotationOffset;
                controller.config = config;
                controller.parentWeapon = weapon;
                controller.rb = bulletInstance.GetComponent<Rigidbody2D>();

                // tell it when to die
                controller.SetDeath();

                return bulletInstance;*/
        return null;
    }

    public static Entity CreateEntity(Entity prefab, LocalToWorld origin, Vector3 offset, Quaternion rotation, Vector3 rotationOffset, BulletSO config, WeaponBase weapon)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // create our bullet instance
        Entity bullet = entityManager.Instantiate(prefab);

        entityManager.SetComponentData(bullet, new Translation { Value = origin.Position+(float3)offset });
        entityManager.SetComponentData(bullet, new Rotation { Value = rotation });
        PhysicsVelocity vel = entityManager.GetComponentData<PhysicsVelocity>(bullet);
        vel.Linear = rotation * Vector3.up * config.baseSpeed;
        entityManager.SetComponentData<PhysicsVelocity>(bullet, vel);

        return bullet;
    }

    protected virtual void Start()
    {
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
    }

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EntityMovementSettings { moveSpeed = config.baseSpeed });
        dstManager.AddComponentData(entity, new LifespanComponent { Value = config.Lifespan });
        dstManager.AddComponentData(entity, new BulletConfigComponent { Damage = config.Damage });

        dstManager.AddComponent(entity, typeof(BulletTag));
    }
}
