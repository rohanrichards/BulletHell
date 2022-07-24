using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour, IConvertGameObjectToEntity
{
    protected StatsController statsController;
    protected Vector3 originalOffset;
    static protected EntityManager entityManager;

    public static Entity CreateEntity(Entity prefab, LocalToWorld origin, Vector3 offset, Quaternion rotation, Quaternion rotationOffset, Vector3 velocityOffset, WeaponSO weaponConfig)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // create our bullet instance
        Entity bullet = entityManager.Instantiate(prefab);

        entityManager.SetComponentData(bullet, new Translation { Value = origin.Position+(float3)offset });
        entityManager.SetComponentData(bullet, new Rotation { Value = rotation });
        entityManager.AddComponentData(bullet, new EntityOffsetData { positionOffset = new Translation { Value = offset }, rotationOffset = new Rotation {Value = rotationOffset } });
        PhysicsVelocity vel = entityManager.GetComponentData<PhysicsVelocity>(bullet);
        vel.Linear = velocityOffset + rotation * Vector3.up * weaponConfig.Speed;
        entityManager.SetComponentData<PhysicsVelocity>(bullet, vel);

        entityManager.AddComponentData(bullet, new EntityMovementSettings { moveSpeed = weaponConfig.Speed });
        entityManager.AddComponentData(bullet, new LifespanComponent { Value = weaponConfig.Lifespan });
        entityManager.AddComponentData(bullet, new BulletConfigComponent { Damage = weaponConfig.Damage, Knockback = weaponConfig.KnockBackForce, DOT = weaponConfig.doesDOT });


        return bullet;
    }

    protected virtual void Start()
    {
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
    }

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(BulletTag));
    }
}
