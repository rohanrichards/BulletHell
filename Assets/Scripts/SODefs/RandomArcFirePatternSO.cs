using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arc Fire Pattern", menuName = "SO's/Fire Patterns/Random Arc")]

public class RandomArcFirePatternSO : FirePatternSO
{
    public override List<Entity> Fire(WeaponSO weaponConfig, Entity bulletPrefab, bool ignoreVelocity = false)
    {
        LocalToWorld playerLocation = ECSPlayerController.getPlayerLocation();
        Vector3 playerVelocity = ECSPlayerController.getPlayerPhysicsVelocity().Linear;
        if (ignoreVelocity) { playerVelocity = Vector3.zero; }
        List<Entity> bullets = new List<Entity>();

        float arcSize = weaponConfig.Spread;
        for (int i = 0; i < weaponConfig.ProjectileCount; i++)
        {

            float rotationOffset = Random.Range(0, arcSize);
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