using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExplosionAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LightDataComponent { radius = 1, color = { x = 1, y = 0.75f, z = 1f }, intensity = 0.75f });
        dstManager.AddComponentData(entity, new DelayedDestroyComponent { Value = 0.1f});
    }
}
