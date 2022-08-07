using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FlailOrbAuthoringComponent : BulletBase
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
        dstManager.AddComponent(entity, typeof(FlailTag));
    }
}
