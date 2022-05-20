using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PickupBase : MonoBehaviour, IConvertGameObjectToEntity
{
    static protected EntityManager entityManager;
    protected StatsController statsController;
    public static Entity Create(Entity prefab, float3 origin)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity pickupInstance = entityManager.Instantiate(prefab);
        entityManager.SetComponentData(pickupInstance, new Translation { Value = origin });

        return pickupInstance;
    }

    public virtual void Start()
    {
        statsController = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
    }

    public virtual void Pickup() { }

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(PickupTag));
    }
}
