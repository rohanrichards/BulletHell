using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

public partial class ReduceSpeedOverTimeSystem : SystemBase
{
    private EntityQuery entityQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<SpeedReducerTag>(),
            ComponentType.ReadWrite<EntityMovementSettings>()
        );
    }

    [BurstCompile]
    partial struct SpeedReducerJob : IJobEntity
    {
        public void Execute(Entity entity, in SpeedReducerTag tag, ref EntityMovementSettings movementData)
        {
            if(movementData.moveSpeed > 0)
            {
                movementData.moveSpeed -= 0.1f;
            }
        }
    }

    protected override void OnUpdate()
    {
        SpeedReducerJob moveJob = new SpeedReducerJob();
        moveJob.Schedule(entityQuery);
    }
}