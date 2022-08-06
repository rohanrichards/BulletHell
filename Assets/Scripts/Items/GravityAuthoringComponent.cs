using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GravityAuthoringComponent : BulletBase
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
        dstManager.AddComponent(entity, typeof(GravityTag));
        dstManager.AddComponent(entity, typeof(SpeedReducerTag));
        dstManager.AddComponentData(entity, new LightDataComponent { radius = 0.5f, color = { x = 1.0f, y = 1.0f, z = 1.0f }, intensity = -1.0f });
    }
}
