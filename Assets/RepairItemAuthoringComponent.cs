using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RepairItemAuthoringComponent : PickupBase, IConvertGameObjectToEntity
{
    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);
        dstManager.AddComponent(entity, typeof(PickupTag));
        dstManager.AddComponentData(entity, new LightDataComponent { radius = 0.5f, color = { x = 0f, y = 0f, z = 1f }, intensity = 20 });

    }
}
