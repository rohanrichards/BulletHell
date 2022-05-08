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
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        float arcSize = 90;
        float arcSegment = arcSize / ProjectileCount;
        float offsetWidth = 0.75f;
        float offsetSegment = offsetWidth / ProjectileCount;
        for (int i = 0; i < ProjectileCount; i++)
        {

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            Vector3 originOffset = playerLocation.Up + (playerLocation.Right * ((offsetWidth / 2) - offset));
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletEntityPrefab, playerLocation, originOffset, Quaternion.Euler(rotation) * playerLocation.Rotation, offsetVector, bulletConfig, this);

            EntityDataComponent data = manager.GetComponentData<EntityDataComponent>(bullet);
            data.Force = KnockBackForce;
            manager.SetComponentData(bullet, data);

            PhysicsVelocity vel = manager.GetComponentData<PhysicsVelocity>(bullet);
            vel.Linear += ECSPlayerController.getPlayerPhysicsVelocity().Linear;
            manager.SetComponentData(bullet, vel);
        }
        yield return new WaitForSeconds(1 / RateOfFire);
        StartCoroutine(Fire());
    }
}
