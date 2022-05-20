using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static EntityMessagingController;

public partial class PickupSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem ecbs;
    private StepPhysicsWorld stepPhysicsWorld;

    [BurstCompile]
    struct PickupOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PickupTag> pickupTag;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playerGroup;
        [ReadOnly] public ComponentDataFromEntity<EntityDataComponent> dataGroup;
        public EntityCommandBuffer ecb;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            Entity pickup;
            Entity player;

            if (pickupTag.HasComponent(entityA) && playerGroup.HasComponent(entityB))
            {
                pickup = entityA;
                player = entityB;
            }
            else if (pickupTag.HasComponent(entityB) && playerGroup.HasComponent(entityA))
            {
                pickup = entityB;
                player = entityA;
            }
            else
            {
                return;
            }

            EntityDataComponent data = dataGroup[pickup];
            Entity message = ecb.CreateEntity();
            ecb.AddComponent(message, new MessageDataComponent { type = MessageTypes.Pickup });
            ecb.AddComponent(message, data);
            ecb.DestroyEntity(pickup);
        }
    }

    protected override void OnCreate()
    {
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        Dependency = new PickupOnTriggerJob
        {
            pickupTag = GetComponentDataFromEntity<PickupTag>(true),
            playerGroup = GetComponentDataFromEntity<PlayerTag>(true),
            dataGroup = GetComponentDataFromEntity<EntityDataComponent>(true),
            ecb = ecb
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
