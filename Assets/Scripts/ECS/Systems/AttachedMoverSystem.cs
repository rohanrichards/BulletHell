using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

public partial class AttachedMoverSystem : SystemBase
{
    private EntityQuery entityQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<AttachToTargetComponent>()
        );
    }

    [BurstCompile]
    partial struct MoveAttachedEntityJob: IJobEntity
    {
        public ComponentDataFromEntity<Translation> translationsGroup;

        public void Execute(Entity entity, in AttachToTargetComponent targetData, in EntityOffsetData offsetData, in Rotation rotation)
        {
            float3 forward = math.mul(rotation.Value, offsetData.positionOffset.Value);
            float3 offset = translationsGroup[targetData.target].Value + (forward);
            translationsGroup[entity] = new Translation { Value = offset };
        }
    }

    protected override void OnUpdate()
    {
        MoveAttachedEntityJob moveJob = new MoveAttachedEntityJob { translationsGroup = GetComponentDataFromEntity<Translation>(false)};
        moveJob.Schedule(entityQuery);
    }
}