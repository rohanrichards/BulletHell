using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

public partial class AttachedRotatorSystem : SystemBase
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
        public ComponentDataFromEntity<Rotation> rotationsGroup;

        public void Execute(Entity entity, in AttachToTargetComponent targetData, in EntityOffsetData offsetData)
        {
            Rotation newRotation = new Rotation { Value = math.mul(rotationsGroup[targetData.target].Value, offsetData.rotationOffset.Value) };
            rotationsGroup[entity] = newRotation;
        }
    }

    protected override void OnUpdate()
    {
        MoveAttachedEntityJob moveJob = new MoveAttachedEntityJob { rotationsGroup = GetComponentDataFromEntity<Rotation>(false) };
        moveJob.Schedule(entityQuery);
    }
}