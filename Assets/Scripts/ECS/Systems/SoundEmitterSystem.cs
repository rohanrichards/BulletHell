using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using FMOD.Studio;
using System.Collections.Generic;

[UpdateAfter(typeof(LifespanReducerSystem))]
[UpdateBefore(typeof(EntityExpirationSystem))]
public partial class SoundEmitterSystem : SystemBase
{
    public List<FMOD.GUID> toCreate = new List<FMOD.GUID>();
    private NativeList<Entity> toStop;
    public Dictionary<Entity, EventInstance> eventInstances = new Dictionary<Entity, EventInstance>();

    [BurstCompile]
    partial struct FindDeadEmittersJob : IJobEntity
    {
        [WriteOnly]
        public NativeList<Entity> toStop;
        public void Execute(Entity entity, ref LifespanComponent lifespanComponent, in SoundEmitterTag tag)
        {
            if(lifespanComponent.Value <= 0)
            {
                toStop.Add(entity);
            }
        }
    }

    private EntityQuery soundEmitterQuery;
    protected override void OnStartRunning()
    {
        soundEmitterQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<SoundEmitterTag>(),
            ComponentType.ReadOnly<LifespanComponent>()
        );
    }

    protected override void OnUpdate()
    {
        NativeArray<Translation> emitterPositions = soundEmitterQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<LifespanComponent> lifespanComponents = soundEmitterQuery.ToComponentDataArray<LifespanComponent>(Allocator.Temp);
        NativeArray<Entity> entities = soundEmitterQuery.ToEntityArray(Allocator.Temp);
        Translation[] positionsList = emitterPositions.ToArray();
        for (int i = 0; i < toCreate.Count; i++)
        {
            Translation translation = emitterPositions[i];
            Entity entity = entities[i];
            FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(toCreate[i]);
            eventInstances.Add(entity, eventInstance);

            FMOD.VECTOR position = new FMOD.VECTOR { x = translation.Value.x, y = translation.Value.y, z = translation.Value.z };
            FMOD.VECTOR forward = new FMOD.VECTOR { x = 0, y = 1, z = 0 };
            FMOD.VECTOR up = new FMOD.VECTOR { x = 0, y = 0, z = 1 };
            FMOD.VECTOR velocity = new FMOD.VECTOR { x = 0, y = 0, z = 0 };
            FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D { position = position, forward = forward, up = up, velocity = velocity };
            eventInstance.set3DAttributes(attributes);

            eventInstance.start();
            eventInstance.release();
        }
        toCreate.Clear();

        for (int i = 0; i < entities.Length; i++)
        {
            Entity entity = entities[i];
            Translation translation = emitterPositions[i];
            if (eventInstances.ContainsKey(entity))
            {
                EventInstance eventInstance = eventInstances[entity];
                FMOD.VECTOR position = new FMOD.VECTOR { x = translation.Value.x, y = translation.Value.y, z = 0 };
                FMOD.VECTOR forward = new FMOD.VECTOR { x = 0, y = 1, z = 0 };
                FMOD.VECTOR up = new FMOD.VECTOR { x = 0, y = 0, z = 1 };
                FMOD.VECTOR velocity = new FMOD.VECTOR { x = 0, y = 0, z = 0 };
                FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D { position = position, forward = forward, up = up, velocity = velocity };
                eventInstance.set3DAttributes(attributes);
            }
        }

        toStop = new NativeList<Entity>(Allocator.TempJob);
        //run the job pass in a native list
        FindDeadEmittersJob updateEmitterPositionJob = new FindDeadEmittersJob { toStop = toStop };
        updateEmitterPositionJob.Schedule().Complete();

        //stop all of the sounds attached to these entities
        for (int i = 0; i < toStop.Length; i++)
        {
            Entity entity = entities[i];
            EventInstance eventInstance = eventInstances[entity];
            if (eventInstances.ContainsKey(entity))
            {
                EventInstance toStopInstance = eventInstances[entity];
                toStopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
        toStop.Dispose();
    }
}
