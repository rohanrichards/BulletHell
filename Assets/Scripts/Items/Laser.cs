using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class Laser : WeaponBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
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

        float arcSize = 50 + (45 * bulletConfig.AOE);
        float arcSegment = arcSize / ProjectileCount;
        float offsetWidth = 0.75f;
        float offsetSegment = offsetWidth / ProjectileCount;
        for (int i = 0; i < ProjectileCount; i++)
        {

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            Vector3 originOffset = playerLocation.Up + (playerLocation.Right * ((offsetWidth / 2) - offset));
            Vector3 rotationOrigin = ((Quaternion)playerLocation.Rotation).eulerAngles;
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = rotationOrigin + new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletEntityPrefab, playerLocation, originOffset, Quaternion.Euler(rotation), offsetVector, bulletConfig, this);

            PhysicsVelocity vel = manager.GetComponentData<PhysicsVelocity>(bullet);
            vel.Linear += ECSPlayerController.getPlayerPhysicsVelocity().Linear;
            manager.SetComponentData(bullet, vel);

            BulletConfigComponent config = manager.GetComponentData<BulletConfigComponent>(bullet);
            config.Knockback = KnockBackForce;
            manager.SetComponentData(bullet, config);
        }
        yield return new WaitForSeconds(1 / RateOfFire);
        StartCoroutine(Fire());
    }
}
