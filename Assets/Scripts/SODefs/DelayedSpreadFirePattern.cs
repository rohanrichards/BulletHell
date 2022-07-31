using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spread Fire Pattern", menuName = "SO's/Fire Patterns/Spread")]

public class DelayedSpreadFirePattern : FirePatternSO
{
    public float arcSize = 10f;
    public float radius = 0.5f;
    public override List<Entity> Fire(WeaponSO weaponConfig, Entity bulletPrefab, bool ignoreVelocity = false)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        float arcSize = weaponConfig.Spread;
        float arcSegment = arcSize / weaponConfig.ProjectileCount;
        float offsetWidth = 0.75f;
        float offsetSegment = offsetWidth / weaponConfig.ProjectileCount;
        List<Entity> bullets = new List<Entity>();

        for (int i = 0; i < weaponConfig.ProjectileCount; i++)
        {
            LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
            Vector3 playerVelocity = ECSPlayerController.getPlayerPhysicsVelocity().Linear;

            float rotationOffset = (arcSegment / 2) + (arcSegment * Random.Range(0, weaponConfig.ProjectileCount));
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            Vector3 originOffset = playerLocation.Up + (playerLocation.Right * ((offsetWidth / 2) - offset));
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletPrefab, playerLocation, originOffset, Quaternion.Euler(rotation) * playerLocation.Rotation, Quaternion.Euler(offsetVector), playerVelocity, weaponConfig);

            LifespanComponent lifespan = manager.GetComponentData<LifespanComponent>(bullet);
            lifespan.Value += Random.Range(0, weaponConfig.Lifespan);
            manager.SetComponentData(bullet, lifespan);

            EntityDataComponent type = new EntityDataComponent { Type = EntityDeathTypes.ExplodesOnDeath, Damage = weaponConfig.Damage, Size = weaponConfig.AOE, Force = weaponConfig.KnockBackForce };
            manager.AddComponentData(bullet, type);

            bullets.Add(bullet);
        }
        return bullets;
    }
}