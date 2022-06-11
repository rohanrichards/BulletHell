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
    }
}
