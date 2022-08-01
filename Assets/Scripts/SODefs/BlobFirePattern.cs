using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blob Fire Pattern", menuName = "SO's/Fire Patterns/Blob")]

public class BlobFirePattern : FirePatternSO
{
    public float arcSize = 10f;
    public float radius = 0.5f;
    public override List<Entity> Fire(WeaponSO weaponConfig, Entity bulletPrefab, bool ignoreVelocity = false)
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        Vector3 playerVelocity = ECSPlayerController.getPlayerPhysicsVelocity().Linear;
        List<Entity> bullets = new List<Entity>();

        float arcSegment = arcSize / weaponConfig.ProjectileCount;
        for (int i = 0; i < weaponConfig.ProjectileCount; i++)
        {
            Vector2 circleposition = Random.insideUnitCircle * radius;

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            Vector3 originOffset = new Vector3(circleposition.x, circleposition.y, 0);
            Vector3 rotationOrigin = ((Quaternion)playerLocation.Rotation).eulerAngles;
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = rotationOrigin + new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletPrefab, playerLocation, originOffset, Quaternion.Euler(rotation), Quaternion.Euler(offsetVector), playerVelocity, weaponConfig);
            bullets.Add(bullet);
        }
        return bullets;
    }
}