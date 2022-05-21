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
public partial class BoidSystem : SystemBase
{
    public NativeArray<Translation> locations;
    public NativeArray<PhysicsVelocity> velocities;
    private EntityQuery entityQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadOnly<BoidTag>()
        );
    }

    protected override void OnUpdate()
    {
        NativeArray<Translation> locations = entityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<PhysicsVelocity> velocities = entityQuery.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp);
        float outerRadius = 10;
        float innerRadius = 3;
        float outerCircleSquare = outerRadius * outerRadius;
        float innerCircleSquare = innerRadius * innerRadius;

        for (int i = 0; i < locations.Length; i++)
        {
            var originLocation = locations[i];
            PhysicsVelocity originVelocity = velocities[i];
            var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var world = physicsWorldSystem.PhysicsWorld;
            NativeList<int> hitsIndices = new NativeList<int>(Allocator.Temp);
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = 1 << 0,
                CollidesWith = 1 << 0,
                GroupIndex = 0
            };
            Aabb aabb = new Aabb
            {
                Min = originLocation.Value + new float3(-outerRadius, -outerRadius, -1),
                Max = originLocation.Value + new float3(outerRadius, outerRadius, 1)
            };
            OverlapAabbInput overlapAabbInput = new OverlapAabbInput
            {
                Aabb = aabb,
                Filter = filter,
            };

            collisionWorld.OverlapAabb(overlapAabbInput, ref hitsIndices);
            var bodies = collisionWorld.Bodies;

            float3 sumLocation = new float3(0, 0, 0);
            float3 sumVelocity = new float3(0, 0, 0);
            float3 acceleration = new float3(0, 0, 0);
            int sumCount = 0;
            int selfRigidBodyIndex = -1;

            for (int z = 0; z < hitsIndices.Length; z++)
            {
                int rigidBodyIndex = hitsIndices[z];
                RigidBody rigidBody = bodies[rigidBodyIndex];
                Entity entity = rigidBody.Entity;
                PhysicsVelocity targetVelocity = GetComponent<PhysicsVelocity>(entity);
                Translation targetLocation = GetComponent<Translation>(entity);
                float3 distanceDelta = targetLocation.Value - originLocation.Value;
                float distancesq = math.lengthsq(distanceDelta);

                if(distancesq == 0)
                {
                    selfRigidBodyIndex = rigidBodyIndex;
                    continue;
                }

                if (distancesq <= outerCircleSquare)
                {
                    sumCount++;
                    sumLocation += targetLocation.Value;
                    sumVelocity += targetVelocity.Linear;

                    //Debug.Log(distancesq + " - " + distanceDelta);

                    if (distancesq <= innerCircleSquare)
                    {
                        float distance = math.sqrt(distancesq);
                        //Debug.Log("within inner circle: " + distance + " - " + acceleration);
                        float3 direction = distanceDelta * (1 / distance);
                        acceleration += -0.002f * direction * (1 / distancesq);
                    }
                }
            }
            if (sumCount > 0)
            {
                float div = 1.0f / sumCount;
                //Debug.Log("final step -- sumVelocity: " + sumVelocity + " - originVelocity: " + originVelocity.Linear + " multiplying by: " + div);
                sumLocation *= div;
                sumVelocity *= div;
                //Debug.Log("New velocity: " + sumVelocity);
                acceleration += math.normalize(sumLocation - originLocation.Value) * 0.002f;
                if((sumVelocity + originVelocity.Linear).Equals(float3.zero))
                {
                    return;
                }
                acceleration += math.normalize(sumVelocity - originVelocity.Linear) * 0.002f;
            }

            //Debug.Log("acceleration: " + acceleration);
            PhysicsWorldExtensions.ApplyLinearImpulse(world, selfRigidBodyIndex, acceleration);
        }
    }
}