using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arc Fire Pattern", menuName = "SO's/Fire Patterns/Arc")]

public class ArcFirePatternSO : FirePatternSO
{
    public override List<Entity> Fire(WeaponSO weaponConfig, Entity bulletPrefab)
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        Vector3 playerVelocity = ECSPlayerController.getPlayerPhysicsVelocity().Linear;
        List<Entity> bullets = new List<Entity>();

        float arcSize = weaponConfig.Spread;
        float arcSegment = arcSize / weaponConfig.ProjectileCount;
        float offsetWidth = 0.75f;
        float offsetSegment = offsetWidth / weaponConfig.ProjectileCount;
        for (int i = 0; i < weaponConfig.ProjectileCount; i++)
        {

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            //Vector3 originOffset = playerLocation.Up + (playerLocation.Right * ((offsetWidth / 2) - offset));
            Vector3 originOffset = Vector3.zero;
            Vector3 rotationOrigin = ((Quaternion)playerLocation.Rotation).eulerAngles;
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = rotationOrigin + new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Entity bullet = BulletBase.CreateEntity(bulletPrefab, playerLocation, originOffset, Quaternion.Euler(rotation), Quaternion.Euler(offsetVector), playerVelocity, weaponConfig);
            bullets.Add(bullet);
        }
        return bullets;
    }
}