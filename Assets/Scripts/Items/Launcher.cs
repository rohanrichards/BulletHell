using System.Collections;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;

public class Launcher : WeaponBase
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Unlock()
    {
        base.Unlock();
    }

    public override IEnumerator Fire()
    {
        float arcSize = weaponConfig.Spread;
        float arcSegment = arcSize / weaponConfig.ProjectileCount;
        float offsetWidth = 0.75f;
        float offsetSegment = offsetWidth / weaponConfig.ProjectileCount;
        for (int i = 0; i < weaponConfig.ProjectileCount; i++)
        {
            LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
            Vector3 playerVelocity = ECSPlayerController.getPlayerPhysicsVelocity().Linear;

            float rotationOffset = (arcSegment / 2) + (arcSegment * Random.Range(0, weaponConfig.ProjectileCount));
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            Vector3 originOffset = playerLocation.Up + (playerLocation.Right * ((offsetWidth / 2) - offset));
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletEntityPrefab, playerLocation, originOffset, Quaternion.Euler(rotation) * playerLocation.Rotation, offsetVector, playerVelocity, weaponConfig);

            LifespanComponent lifespan = manager.GetComponentData<LifespanComponent>(bullet);
            lifespan.Value += Random.Range(0, weaponConfig.Lifespan);
            manager.SetComponentData(bullet, lifespan);

            EntityDataComponent type = new EntityDataComponent { Type = EntityDeathTypes.ExplodesOnDeath, Damage = weaponConfig.Damage, Size = weaponConfig.AOE, Force = weaponConfig.KnockBackForce };
            manager.AddComponentData(bullet, type);

            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1 / weaponConfig.ROF);
        StartCoroutine(Fire());
    }
}
