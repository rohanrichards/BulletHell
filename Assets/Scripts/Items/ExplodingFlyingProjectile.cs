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

        dstManager.AddComponent(entity, typeof(MoveForwardTag));
    }
}
