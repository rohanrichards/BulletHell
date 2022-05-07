using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics.Extensions;
using UnityEngine;

public class EntityMessagingController : MonoBehaviour
{
    public GameObject bulletDeathPrefab;
    EntityManager manager;
    private EntityQuery entityQuery;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        entityQuery = manager.CreateEntityQuery(
            ComponentType.ReadWrite<MessageDataComponent>(),
            ComponentType.ReadWrite<EntityTypeComponent>()
        );
    }

    // Update is called once per frame
    void Update()
    {
        NativeArray<MessageDataComponent> entityData = entityQuery.ToComponentDataArray<MessageDataComponent>(Allocator.Temp);
        NativeArray<EntityTypeComponent> typeData = entityQuery.ToComponentDataArray<EntityTypeComponent>(Allocator.Temp);
        for (int i = 0; i < entityData.Length; i++)
        {
            MessageDataComponent message = entityData[i];
            EntityTypeComponent type = typeData[i];

            if(message.type == 100)
            {
                //death message
                Instantiate(bulletDeathPrefab, message.position, message.rotation);
                Entity exploder = manager.CreateEntity();
                manager.AddComponent<ExplodeAndDeleteTag>(exploder);
                manager.AddComponent<Translation>(exploder);
                manager.SetComponentData<Translation>(exploder, new Translation { Value = message.position });
            }
        }
        manager.DestroyEntity(entityQuery);
    }
}
