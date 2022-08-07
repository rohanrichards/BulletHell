using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

// [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
// [UpdateAfter(typeof(BuildPhysicsWorld))]
// [UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class FlailSystem : SystemBase
{
    public NativeArray<Translation> locations;
    private EntityQuery entityQuery;
    private EntityQuery playerQuery;
    private EndSimulationEntityCommandBufferSystem ecbs;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadWrite<BulletConfigComponent>(),
            ComponentType.ReadWrite<FlailTag>()
        );
        playerQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<PlayerTag>()
        );
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    partial struct FlailJob : IJobEntity
    {
        public float3 playerPos;
        // public PhysicsWorld physicsWorld;
        // public float deltaTime;

        public void Execute(Entity entity, in FlailTag tag, in Translation location, in BulletConfigComponent config, ref PhysicsVelocity vel)
        {
            float slackDist = config.Size * 5.0f;
            float forceMult = config.Knockback;

            float3 playerDelta = playerPos - location.Value;
            float dist = math.length(playerDelta);

            if(dist > slackDist) {
                float force = (slackDist - dist);
                force = force * force;
                playerDelta *= 1.0f / dist;

                vel.Linear += playerDelta * force * forceMult;
            }
            // int physicsIndex = physicsWorld.GetRigidBodyIndex(entity);
            // PhysicsWorldExtensions.ApplyLinearImpulse(physicsWorld, physicsIndex, goalDelta * deltaTime * 1.01f);
        }
    }

    protected override void OnUpdate()
    {
        NativeArray<Translation> playerLocations = playerQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        var physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;

        var deltaTime = Time.DeltaTime;
        FlailJob flailJob = new FlailJob{
            playerPos = playerLocations[0].Value//,
            // physicsWorld = physicsWorld,
            // deltaTime = deltaTime
        };
        flailJob.Schedule(entityQuery);
    }
}