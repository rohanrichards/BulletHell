using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CoinAuthoringComponent : PickupBase, IConvertGameObjectToEntity
{
    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);
    }
}
