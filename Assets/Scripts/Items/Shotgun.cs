using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class Shotgun : WeaponBase
{
    public float arcSize = 10f;
    public float radius = 0.5f;
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
        Vector3 playerVelocity = ECSPlayerController.getPlayerPhysicsVelocity().Linear;

        float arcSegment = arcSize / weaponConfig.ProjectileCount;
        for (int i = 0; i < weaponConfig.ProjectileCount; i++)
        {
            Vector2 circleposition = Random.insideUnitCircle * radius;

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            Vector3 originOffset = new Vector3(circleposition.x, circleposition.y, 0);
            Vector3 rotationOrigin = ((Quaternion)playerLocation.Rotation).eulerAngles;
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = rotationOrigin + new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletEntityPrefab, playerLocation, originOffset, Quaternion.Euler(rotation), offsetVector, playerVelocity, weaponConfig);
        }
        yield return new WaitForSeconds(1 / weaponConfig.ROF);
        StartCoroutine(Fire());
    }
}
