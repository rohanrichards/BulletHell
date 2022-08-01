using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class LaserBeamAuthoringComponent : BulletBase
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        EntityDataComponent type = new EntityDataComponent { Type = EntityDeathTypes.DoesNothingOnDeath };
        dstManager.AddComponentData(entity, type);

        dstManager.AddComponent(entity, typeof(DeparentTag));
        dstManager.RemoveComponent<MoveForwardTag>(entity);
    }
}
