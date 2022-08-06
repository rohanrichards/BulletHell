using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BuildPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class GravitySystem : SystemBase
{
    public NativeArray<Translation> locations;
    private EntityQuery entityQuery;
    private EntityQuery playerQuery;
    private EndSimulationEntityCommandBufferSystem ecbs;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<BulletConfigComponent>(),
            ComponentType.ReadWrite<GravityTag>()
        );
        playerQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<PlayerTag>()
        );
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        NativeArray<Translation> locations = entityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<Translation> playerLocations = playerQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<BulletConfigComponent> datas = entityQuery.ToComponentDataArray<BulletConfigComponent>(Allocator.Temp);

        for (int i = 0; i < locations.Length; i++)
        {
            var location = locations[i];
            var data = datas[i];
            var playerLocation = playerLocations[0];
            var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var world = physicsWorldSystem.PhysicsWorld;
            var size = data.Size * 5.0f;
            NativeList<int> hitsIndices = new NativeList<int>(Allocator.Temp);
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = 1 << 1,
                CollidesWith = 1 << 0,
                GroupIndex = 0
            };
            Aabb aabb = new Aabb
            {
                Min = location.Value + new float3(-size, -size, -1),
                Max = location.Value + new float3(size, size, 1)
            };
            OverlapAabbInput overlapAabbInput = new OverlapAabbInput
            {
                Aabb = aabb,
                Filter = filter,
            };

            collisionWorld.OverlapAabb(overlapAabbInput, ref hitsIndices);
            var bodies = collisionWorld.Bodies;
            var sqrRadius = size * size;

            for (int z = 0; z < hitsIndices.Length; z++)
            {
                int rigidBodyIndex = hitsIndices[z];
                RigidBody rigidBody = bodies[rigidBodyIndex];
                Entity entity = rigidBody.Entity;
                Translation position = GetComponent<Translation>(entity);
                float3 distanceOfPlayer = playerLocation.Value - location.Value;
                float3 directionOfPlayer = math.normalize(distanceOfPlayer);
                if (math.lengthsq(location.Value - position.Value) <= sqrRadius)
                {
                    float3 direction = position.Value - location.Value;
                    float falloff = 1 - math.length(direction) / size;
                    float3 force = math.normalize(direction) * -data.Knockback * falloff * Time.DeltaTime;
                    PhysicsWorldExtensions.ApplyLinearImpulse(world, rigidBodyIndex, force);
                }
            }
        }
    }
}