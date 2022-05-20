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

[UpdateBefore(typeof(SimulationSystemGroup))]
public partial class PickupMagnetSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem ecbs;
    private StepPhysicsWorld stepPhysicsWorld;

    [BurstCompile]
    struct MoveOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<MagneticPickupTag> pickupTag;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playerGroup;
        [ReadOnly] public ComponentDataFromEntity<Translation> translationsGroup;
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
            } else if (pickupTag.HasComponent(entityB) && playerGroup.HasComponent(entityA))
            {
                pickup = entityB;
                player = entityA;
            }else
            {
                return;
            }

            Translation playerPosition = translationsGroup[player];
            Translation pickupPosition = translationsGroup[pickup];
            float3 diff = pickupPosition.Value - playerPosition.Value;
            float len = math.length(diff);


            if (len <= 0.75)
            {
                EntityDataComponent data = dataGroup[pickup];
                Entity message = ecb.CreateEntity();
                ecb.AddComponent(message, new MessageDataComponent { type = MessageTypes.Pickup });
                ecb.AddComponent(message, data);
                ecb.DestroyEntity(pickup);
            }else
            {
                ecb.AddComponent(pickup, new EntityMovementSettings { moveSpeed = 250 });
                ecb.AddComponent<MoveTowardTargetTag>(pickup);
            }
        }
    }

    protected override void OnCreate()
    {
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        Dependency = new MoveOnTriggerJob
        {
            pickupTag = GetComponentDataFromEntity<MagneticPickupTag>(true),
            playerGroup = GetComponentDataFromEntity<PlayerTag>(true),
            translationsGroup = GetComponentDataFromEntity<Translation>(true),
            dataGroup = GetComponentDataFromEntity<EntityDataComponent>(true),
            ecb = ecb
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);

        ecbs.AddJobHandleForProducer(Dependency);

    }
}
