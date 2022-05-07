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

    public enum MessageTypes {
        Death = 100
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        entityQuery = manager.CreateEntityQuery(
            ComponentType.ReadWrite<MessageDataComponent>(),
            ComponentType.ReadWrite<EntityDataComponent>()
        );
    }

    // Update is called once per frame
    void Update()
    {
/*        NativeArray<MessageDataComponent> entityData = entityQuery.ToComponentDataArray<MessageDataComponent>(Allocator.Temp);
        NativeArray<EntityDataComponent> typeData = entityQuery.ToComponentDataArray<EntityDataComponent>(Allocator.Temp);
        for (int i = 0; i < entityData.Length; i++)
        {
            MessageDataComponent message = entityData[i];
            EntityDataComponent type = typeData[i];

            if(message.type == MessageTypes.Death)
            {
                if(type.Type == EntityTypes.ExplodesOnDeath)
                {
                    Instantiate(bulletDeathPrefab, message.position, message.rotation);
                    Entity exploder = manager.CreateEntity();
                    manager.AddComponent<ExplodeAndDeleteTag>(exploder);

                    manager.AddComponent<EntityDataComponent>(exploder);
                    manager.SetComponentData(exploder, type);

                    manager.AddComponent<Translation>(exploder);
                    manager.SetComponentData(exploder, new Translation { Value = message.position });
                }
            }
        }
        manager.DestroyEntity(entityQuery);*/
    }
}
