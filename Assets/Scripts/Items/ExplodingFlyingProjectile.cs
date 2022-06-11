using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExplodingFlyingProjectile : BulletBase, IConvertGameObjectToEntity
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        EntityDataComponent type = new EntityDataComponent { Type = EntityDeathTypes.ExplodesOnDeath, Damage = config.Damage, Size = config.AOE };
        dstManager.AddComponentData(entity, type);
        dstManager.AddComponent(entity, typeof(MoveForwardTag));
    }
}
