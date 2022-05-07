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
public partial class ExplosionSystem : SystemBase
{
    public NativeArray<Translation> locations;
    private EntityQuery entityQuery;
    private EndSimulationEntityCommandBufferSystem ecbs;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<ExplodeAndDeleteTag>()
        );
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        int radius = 3;
        NativeArray<Translation> locations = entityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        for (int i = 0; i < locations.Length; i++)
        {
            var location = locations[i];
            var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var world = physicsWorldSystem.PhysicsWorld;
            NativeList<int> hitsIndices = new NativeList<int>(Allocator.Temp);
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = 1 << 1,
                CollidesWith = 1 << 0,
                GroupIndex = 0
            };
            Aabb aabb = new Aabb
            {
                Min = location.Value + new float3(-radius, -radius, -1),
                Max = location.Value + new float3(radius, radius, 1)
            };
            OverlapAabbInput overlapAabbInput = new OverlapAabbInput
            {
                Aabb = aabb,
                Filter = filter,
            };

            collisionWorld.OverlapAabb(overlapAabbInput, ref hitsIndices);
            var bodies = collisionWorld.Bodies;
            var sqrRadius = radius * radius;

            for (int z = 0; z < hitsIndices.Length; z++)
            {
                int rigidBodyIndex = hitsIndices[z];
                RigidBody rigidBody = bodies[rigidBodyIndex];
                Translation position = GetComponent<Translation>(rigidBody.Entity);
                if (math.lengthsq(location.Value - position.Value) <= sqrRadius)
                {
                    float3 direction = position.Value - location.Value;
                    float falloff = radius / math.length(direction);
                    float3 force = math.normalize(direction) * 1 * falloff * Time.DeltaTime;
                    PhysicsWorldExtensions.ApplyLinearImpulse(world, rigidBodyIndex, force);
                }
            }
        }
        EntityManager.DestroyEntity(entityQuery);
    }
}