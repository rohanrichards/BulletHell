using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Entities;

public class ChestPickupAuthoringComponent : PickupBase, IConvertGameObjectToEntity
{
    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);
        dstManager.AddComponent(entity, typeof(PickupTag));
        dstManager.AddComponentData(entity, new LightDataComponent { radius = 0.5f, color = { x = 1f, y = 0.75f, z = 0.1f }, intensity = 10 });
    }
}
