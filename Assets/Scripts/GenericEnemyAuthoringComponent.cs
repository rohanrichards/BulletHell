using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class GenericEnemyAuthoringComponent : EnemyBase, IConvertGameObjectToEntity
{
    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);
        dstManager.AddComponent(entity, typeof(MoveTowardTargetTag));
        dstManager.AddComponent(entity, typeof(RotateToTargetTag));
        //dstManager.AddComponent(entity, typeof(BoidTag));
    }
}
